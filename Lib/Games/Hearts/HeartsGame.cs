using System.Diagnostics;

using CardGamesPrototype.Lib.Common;

using Microsoft.Extensions.Logging;

namespace CardGamesPrototype.Lib.Games.Hearts;

// TODO: Chimera would be a fun one to implement
//      this would be a fun one, esp since it has non-standard shuffling/dealing/betting
//      have a deal func that knows how many hands to deal to, and the optional max capacity of those hands? and/or a priority of the hands to deal to?

// TODO: make a separate IPlayerInterface and leave this as purely concrete

// TODO: a player name would be cool too

// TODO: put try/catch inside game whileLoop so if any excs occur, can continue the game?

// TODO: have a Players : IList<Player> w/ a method to concurrently apply op to all players?
//      have func to play out trick/op, starting with a specific player?

// TODO: have hand sort funcs? order by rank then suit, and order by suit then rank
//      ace low vs high
//      make an actual hand type?

// TODO: how communicate outputs to players? not just a logger, right?

// TODO: save after every trick? after every card placement? only on req?

public sealed class HeartsGame : IGame
{
    private const int NumPlayers = 4;
    private readonly List<HeartsPlayer> _players;
    private readonly IDealer _dealer;
    private readonly ILogger<HeartsGame> _logger;

    private static readonly Player.RemoveCardsSpec PlayerSpecRemove3Cards = new(
        MinCardsToRemove: 3,
        MaxCardsToRemove: 3);

    public sealed class Factory(IDealer dealer, ILogger<HeartsGame> logger)
    {
        public HeartsGame Make(List<Player> players) => new(dealer, players, logger);
    }

    private HeartsGame(IDealer dealer, List<Player> players, ILogger<HeartsGame> logger)
    {
        _dealer = dealer;
        _logger = logger;
        if (players.Count != NumPlayers)
            throw new ArgumentException(
                $"Hearts requires exactly {NumPlayers} players, but given {players.Count}");
        _players = new List<HeartsPlayer>(capacity: NumPlayers);
        foreach (Player player in players)
            _players.Add(new HeartsPlayer(player));
    }

    public async Task Play(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting a game of Hearts");
        CircularCounter cardPassingDirection = new(4, startAtEnd: true);
        while (_players.All(player => player.Score < 100))
        {
            await SetupRound((PassDirection)cardPassingDirection.Tick(), cancellationToken);

            int iCurrPlayer = _players.FindIndex(heartsPlayer =>
                heartsPlayer.PeakCards.Contains(TwoOfClubs.Instance));
            if (iCurrPlayer == -1)
                throw new InvalidOperationException(
                    $"Could not find a player with the {nameof(TwoOfClubs)}");

            bool hasHeartsBeenBroken = false;
            (iCurrPlayer, hasHeartsBeenBroken) =
                await PlayTrick(iCurrPlayer, hasHeartsBeenBroken, cancellationToken);

            while (_players[0].PeakCards.Count > 0)
            {
                (iCurrPlayer, hasHeartsBeenBroken) =
                    await PlayTrick(iCurrPlayer, hasHeartsBeenBroken, cancellationToken);
            }

            // TODO: count points accrued in tricks (watch out for shooting the moon!)
        }

        _logger.LogInformation("Completed a game of Hearts");
    }

    private Task<(int iNewCurrPlayer, bool hasHeartsBeenBroken)> PlayTrick(
        int iCurrPlayer,
        bool hasHeartsBeenBroken,
        CancellationToken cancellationToken)
    {
        // TODO: this
        //      - if first trick, iCurrPlayer must play 2clubs
        //      - else, ask iCurrPlayer for a card to begin trick
        //          hearts cannot be lead until broken!
        //      move clockwise around players for their card in the trick
        //          players must follow suit if they can
        //          if hearts is broken, track that
        //          if first trick, no points can be played this trick!
        //      whoever has the highest card in the suit takes the trick and becomes iCurrPlayer
        //          ace is high!

        // TODO: impl ideas
        //      pass linq or specs to player?
        //      how best determine b/w first/non-first tricks?

        throw new NotImplementedException();
    }

    private async Task SetupRound(PassDirection passDirection, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Shuffling, cutting, and dealing the deck to {NumPlayers}",
            NumPlayers);
        // TODO: could preserve and reshuffle cards instead of reinstantiating every round
        List<Cards<HeartsCard>> hands = _dealer.ShuffleCutDeal(
            deck: HeartsCard.MakeDeck(Decks.Standard52()),
            numHands: NumPlayers);
        for (int i = 0; i < NumPlayers; i++)
            await _players[i].SetHand(hands[i], cancellationToken);

        if (passDirection is PassDirection.Hold)
        {
            _logger.LogInformation("Hold 'em round! No passing");
            return;
        }

        _logger.LogInformation("Asking each player to select three cards to pass {PassDirection}",
            passDirection);
        List<Task<Cards<HeartsCard>>> takeCardsFromPlayerTasks = new(capacity: NumPlayers);
        for (int i = 0; i < NumPlayers; i++)
        {
            Task<Cards<HeartsCard>> task = _players[i]
                .RemoveCards<HeartsCard>(PlayerSpecRemove3Cards, cancellationToken);
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

            Cards<HeartsCard> cardsToPass = takeCardsFromPlayerTasks[iSourcePlayer].Result;
            Task task = _players[iTargetPlayer].GiveCards(cardsToPass, cancellationToken);
            giveCardsToPlayerTasks.Add(task);
        }

        await Task.WhenAll(giveCardsToPlayerTasks).WaitAsync(cancellationToken);
        _logger.LogInformation("Hands are finalized");
    }
}
