using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace CardGamesPrototype.Lib.Games;

// TODO: have a Players : IList<Player> w/ a method to concurrently apply op to all players?
//      have func to play out trick, starting with specific player?

// TODO: have a hand funcs? order by rank then suit, and order by suit then rank
//      ace low vs high
//      make an actual hand type?

// TODO: how communicate outputs to players? not just a logger, right?

// TODO: save after every trick? after every card placement? only on req?

public sealed class HeartsGame : IGame
{
    public sealed class Factory(ILogger<HeartsGame> logger)
    {
        public HeartsGame Make(List<Player> players) => new HeartsGame(logger, players);
    }

    private sealed record PlayerState(Player Player)
    {
        public int Score { get; set; } = 0;
        public List<Trick> TricksTakenThisRound { get; set; } = [];
    }

    private enum PassDirection
    {
        Left = 0,
        Right = 1,
        Across = 2,
        Hold = 3,
    }

    private const int NumPlayers = 4;
    private readonly List<PlayerState> _playerStates = new(capacity: NumPlayers);

    private readonly ILogger<HeartsGame> _logger;

    public HeartsGame(ILogger<HeartsGame> logger, List<Player> players)
    {
        _logger = logger;
        if (players.Count != NumPlayers)
            throw new ArgumentException(
                $"Hearts requires exactly {NumPlayers} players, but given {players.Count}");

        foreach (Player player in players)
            _playerStates.Add(new PlayerState(player));
    }

    private static readonly Player.RemoveCardsSpec PlayerSpecRemove3Cards = new(
        MinCardsToRemove: 3,
        MaxCardsToRemove: 3);

    public async Task Play(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting a game of Hearts");
        CircularCounter cardPassingDirection = new(4, startAtEnd: true);
        while (_playerStates.All(player => player.Score < 100))
        {
            PassDirection passDirection = (PassDirection)cardPassingDirection.Tick();
            await SetupRound(passDirection, cancellationToken);

            // the player with 2clubs starts the first trick
            //      no points can be played this trick!
            //          may need to *actually* implement the Spec pattern since points are game-specific
            //          unless willing to have game bounce selection back to player?
            // play tricks until everyone runs out of cards
            //      hearts cannot be lead until broken!
            // count points accrued (watch out for shooting the moon!)
            // if no winner, play another round
        }

        _logger.LogInformation("Completed a game of Hearts");
    }

    private async Task SetupRound(PassDirection passDirection, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Shuffling, cutting, and dealing the deck to {NumPlayers}",
            NumPlayers);
        List<Deck> hands = Deck.Make().Shuffle().Cut().Deal(NumPlayers);
        for (int i = 0; i < NumPlayers; i++)
            await _playerStates[i].Player.SetHand(hands[i].GetCards(), cancellationToken);

        if (passDirection is PassDirection.Hold)
        {
            _logger.LogInformation("Hold 'em round! No passing");
            return;
        }

        _logger.LogInformation("Asking each player to select three cards to pass {PassDirection}",
            passDirection);
        List<Task<List<Card>>> takeCardsFromPlayerTasks = new(capacity: NumPlayers);
        for (int i = 0; i < NumPlayers; i++)
        {
            Task<List<Card>> task = _playerStates[i].Player
                .RemoveCards(PlayerSpecRemove3Cards, cancellationToken);
            takeCardsFromPlayerTasks.Add(task);
        }

        await Task.WhenAll(takeCardsFromPlayerTasks).WaitAsync(cancellationToken);

        _logger.LogInformation("Passing cards");
        List<Task> giveCardsToPlayerTasks = new(capacity: NumPlayers);
        for (int iSourcePlayer = 0; iSourcePlayer < NumPlayers; iSourcePlayer++)
        {
            CircularCounter sourcePlayerPosition = new(iSourcePlayer, NumPlayers);
            int iTargetPlayer = passDirection switch
            {
                PassDirection.Left => sourcePlayerPosition.Tick(updateInstance: false),
                PassDirection.Right => sourcePlayerPosition.Tick(-1, updateInstance: false),
                PassDirection.Across => sourcePlayerPosition.Tick(2, updateInstance: false),
                _ => throw new UnreachableException(
                    $"Passing {passDirection} from {nameof(iSourcePlayer)} {iSourcePlayer}"),
            };

            List<Card> cardsToPass = takeCardsFromPlayerTasks[iSourcePlayer].Result;
            Task task = _playerStates[iTargetPlayer].Player
                .GiveCards(cardsToPass, cancellationToken);
            giveCardsToPlayerTasks.Add(task);
        }

        await Task.WhenAll(giveCardsToPlayerTasks).WaitAsync(cancellationToken);
        _logger.LogInformation("Hands are finalized");
    }
}
