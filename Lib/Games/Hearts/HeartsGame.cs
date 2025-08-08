using System.Diagnostics;

using Microsoft.Extensions.Logging;

// TODO: rename Common to AbstractCore?

namespace CardGamesPrototype.Lib.Games.Hearts;

// TODO: have a Players : IList<Player> w/ a method to concurrently apply op to all players?
//      have func to play out trick/op, starting with a specific player?

// TODO: Chimera would be a fun one to implement
//      similar to ichu, The Great Dalmuti, Big Two, and Beat the Landlord
//      this would be a fun one, esp since it has non-standard shuffling/dealing/betting
//      have a deal func that knows how many hands to deal to, and the optional max capacity of those hands? and/or a priority of the hands to deal to?

// TODO: make a separate IPlayerInterface and leave this as purely concrete?

// TODO: a player name would be cool too

// TODO: put try/catch inside game whileLoop so if any excs occur, can continue the game?

// TODO: have hand sort funcs? order by rank then suit, and order by suit then rank
//      ace low vs high
//      make an actual hand type?

// TODO: how communicate outputs to players? not just a logger, right?

// TODO: save after every trick? after every card placement? only on req?

public sealed class HeartsGame : IGame
{
    private const int EndOfGameScore = 100;
    private const int NumPlayers = 4;
    private readonly List<HeartsPlayer> _players;
    private readonly IDealer _dealer;
    private readonly ILogger<HeartsGame> _logger;

    public sealed class Factory(IDealer dealer, ILogger<HeartsGame> logger)
    {
        public HeartsGame Make(List<HeartsPlayer> players) => new(dealer, players, logger);
    }

    private HeartsGame(IDealer dealer, List<HeartsPlayer> players, ILogger<HeartsGame> logger)
    {
        _dealer = dealer;
        _logger = logger;
        if (players.Count != NumPlayers)
            throw new ArgumentException(
                $"Hearts requires exactly {NumPlayers} players, but given {players.Count}");
        _players = players;
    }

    public async Task Play(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting a game of Hearts");
        CircularCounter cardPassingDirection = new(4, startAtEnd: true);
        while (_players.All(player => player.Score < EndOfGameScore))
        {
            await SetupRound((PassDirection)cardPassingDirection.Tick(), cancellationToken);

            int iTrickStartPlayer = _players.FindIndex(player =>
                player.PeakHand.Any(cardValue => cardValue is TwoOfClubs));
            if (iTrickStartPlayer == -1)
                throw new InvalidOperationException(
                    $"Could not find a player with the {nameof(TwoOfClubs)}");

            bool hasHeartsBeenBroken = false;
            (iTrickStartPlayer, hasHeartsBeenBroken) = await PlayOutTrick(
                firstTrick: true,
                iTrickStartPlayer: iTrickStartPlayer,
                hasHeartsBeenBroken: hasHeartsBeenBroken,
                cancellationToken: cancellationToken);

            while (_players[0].PeakHand.Any())
            {
                (iTrickStartPlayer, hasHeartsBeenBroken) = await PlayOutTrick(
                    firstTrick: false,
                    iTrickStartPlayer: iTrickStartPlayer,
                    hasHeartsBeenBroken: hasHeartsBeenBroken,
                    cancellationToken: cancellationToken);
            }

            // TODO: count points accrued in tricks (watch out for shooting the moon!)
        }

        _logger.LogInformation("Completed a game of Hearts");
    }

    private async Task<(int iNewCurrPlayer, bool hasHeartsBeenBroken)> PlayOutTrick(
        bool firstTrick,
        int iTrickStartPlayer,
        bool hasHeartsBeenBroken,
        CancellationToken cancellationToken)
    {
        CircularCounter iTrickPlayer = new(iTrickStartPlayer);
        _logger.LogInformation("Getting trick's opening card from player at position {PlayerPosition}", iTrickPlayer.N);
        HeartsCard openingCard = await _players[iTrickStartPlayer].PlayCard(
            validateChosenCard: (hand, iCardToPlay) => firstTrick
                ? hand[iCardToPlay].Value is TwoOfClubs
                : CheckPlayedHeartsCard.EnsureHeartsArePlayedOnlyAfterBeingBroken(hasHeartsBeenBroken, hand, iCardToPlay),
            cancellationToken);
        _logger.LogInformation("Player at position {PlayerPosition} played {CardValue}", iTrickPlayer.N, openingCard.Value);

        Cards<HeartsCard> trick = new(capacity: NumPlayers) { openingCard };
        while (iTrickPlayer.CycleClockwise() != iTrickStartPlayer)
        {
            _logger.LogInformation("Getting trick's next card from player at position {PlayerPosition}", iTrickPlayer.N);
            HeartsCard chosenCard = await _players[iTrickPlayer.N].PlayCard(
                validateChosenCard: (hand, iCardToPlay) => CheckPlayedCard.EnsureTrickSuitIsFollowedIfPossible(trick, hand, iCardToPlay) && firstTrick
                    ? hand[iCardToPlay].Points == 0
                    : CheckPlayedHeartsCard.EnsureHeartsArePlayedOnlyAfterBeingBroken(hasHeartsBeenBroken, hand, iCardToPlay),
                cancellationToken);
            trick.Add(chosenCard);
            _logger.LogInformation("Player at position {PlayerPosition} played {CardValue}", iTrickPlayer.N, chosenCard.Value);

            if (!hasHeartsBeenBroken && chosenCard.Value.Suit is Suit.Hearts)
            {
                _logger.LogInformation("Hearts has been broken!");
                hasHeartsBeenBroken = true;
            }
        }

        if (trick.Count != NumPlayers)
            throw new InvalidOperationException($"After playing a trick, the trick has {trick.Count} cards but expected {NumPlayers} cards");

        // TODO: this
        //      whoever has the highest card in the suit takes the trick and becomes iCurrPlayer
        //          ace is high!
        int iNewCurrPlayer = -1; // TODO: this
        _logger.LogInformation("Player at position {PlayerPosition} took the trick", iNewCurrPlayer);

        return (iNewCurrPlayer, hasHeartsBeenBroken);
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
            await _players[i].GiveCards(hands[i], cancellationToken);

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
            Task<Cards<HeartsCard>> task = _players[i].PlayCards(
                validateChosenCards: playedCards => playedCards.Count == 3,
                cancellationToken);
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
                PassDirection.Left => sourcePlayerPosition.CycleClockwise(updateInstance: false),
                PassDirection.Right => sourcePlayerPosition.CycleCounterClockwise(updateInstance: false),
                PassDirection.Across => sourcePlayerPosition.CycleClockwise(times: 2, updateInstance: false),
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
