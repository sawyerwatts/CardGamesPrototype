using System.Diagnostics;

using CardGamesPrototype.Lib.Common;

using Microsoft.Extensions.Logging;

namespace CardGamesPrototype.Lib.Games;

// TODO: make a Dealer for the deck so can more conventionally inject/mock rng? and inject deck into dealer?
//      and so could log w/o method injection?

// TODO: Chimera would be a fun one to implement
//      this would be a fun one, esp since it has non-standard shuffling/dealing/betting
//      have a deal func that knows how many hands to deal to, and the optional max capacity of those hands? and/or a priority of the hands to deal to?

// TODO: make a separate IPlayerInterface and leave this as purely concrete

// TODO: a player name would be cool too

// TODO: put try/catch inside game whileLoop so if any excs occur, can continue the game?

// TODO: have a Players : IList<Player> w/ a method to concurrently apply op to all players?
//      have func to play out trick/op, starting with a specific player?

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
        Hold = 3
    }

    private const int NumPlayers = 4;
    private readonly List<PlayerState> _playerStates = new(capacity: NumPlayers);

    private readonly ILogger<HeartsGame> _logger;

    private HeartsGame(ILogger<HeartsGame> logger, List<Player> players)
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
            await SetupRound((PassDirection)cardPassingDirection.Tick(), cancellationToken);

            int iCurrPlayer = _playerStates.FindIndex(playerState =>
                playerState.Player.PeakCards.Contains(TwoOfClubs.Instance));
            if (iCurrPlayer == -1)
                throw new InvalidOperationException(
                    $"Could not find a player with the {nameof(TwoOfClubs)}");

            // TODO: pointing/specification design
            //      have a HeartsDeck w/ HeartsCard
            //      then use IDealer to shuffle/cut/deal that deck
            //      and manage the hands internally
            //      and use Player to communicate with players via here's your whole hand, choose one of these cards
            //      also have DeckHandTrick.cs? or just use lists?

            // TODO: play first trick
            //      no points can be played this trick!
            //          how attribute points?
            //          need a HeartsCard type w/ points?
            //          use linq to inject the valid cards that could be specified?
            //              Player would basically just become IPlayer
            //      may need to *actually* implement the Spec pattern since points are game-specific
            //          unless willing to have game bounce selection back to human player?

            while (_playerStates[0].Player.PeakCards.Count > 0)
            {
                // TODO: play a trick and update iCurrPlayer to trick taker
                //      hearts cannot be lead until broken!
            }

            // TODO: count points accrued in tricks (watch out for shooting the moon!)
        }

        _logger.LogInformation("Completed a game of Hearts");
    }

    private async Task SetupRound(PassDirection passDirection, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Shuffling, cutting, and dealing the deck to {NumPlayers}",
            NumPlayers);
        List<Deck> hands = Deck.Make().Shuffle().Cut().Deal(NumPlayers); // TODO: this makes testing real hard (see top for more)
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
