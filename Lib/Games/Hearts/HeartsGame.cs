using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CardGamesPrototype.Lib.Games.Hearts;

// TODO: this class is hard to test as-written. If this wasn't a warm up to understand building card games, I would refactor this class to make it more testability.

// TODO: have a Players : IList<Player> w/ a method to concurrently apply op to all players?
//      have func to play out trick/op, starting with a specific player?
//      could be useful in notifying other players of a change

// TODO: Chimera would be a fun one to implement
//      similar to ichu, The Great Dalmuti, Big Two, and Beat the Landlord
//      this would be a fun one, esp since it has non-standard shuffling/dealing/betting
//      have a deal func that knows how many hands to deal to, and the optional max capacity of those hands? and/or a priority of the hands to deal to?

// TODO: fuzzing, if not simulation testing all possible permutations of inputs, would be real fun

// TODO: how communicate outputs to players? not just a logger, right?

// TODO: save after every trick? after every card placement? only on req?

public sealed class HeartsGame : IGame
{
    private const int NumPlayers = 4;
    private readonly List<HeartsPlayer> _players;
    private readonly IDealer _dealer;
    private readonly ILogger<HeartsGame> _logger;
    private readonly Options _options;

    public sealed class Factory(IDealer dealer, IOptions<Options> options, ILogger<HeartsGame> logger)
    {
        public HeartsGame Make(List<HeartsPlayer> players) => new(dealer, players, options, logger);
    }

    private HeartsGame(IDealer dealer, List<HeartsPlayer> players, IOptions<Options> options, ILogger<HeartsGame> logger)
    {
        _dealer = dealer;
        _logger = logger;
        _options = options.Value;
        if (players.Count != NumPlayers)
            throw new ArgumentException(
                $"Hearts requires exactly {NumPlayers} players, but given {players.Count}");
        _players = players;
    }

    public async Task Play(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting a game of Hearts");
        CircularCounter dealerPosition = new(4, startAtEnd: true);
        while (_players.All(player => player.Score < _options.EndOfGamePoints))
        {
            await SetupRound((PassDirection)dealerPosition.Tick(), cancellationToken);

            int iTrickStartPlayer = _players.FindIndex(player =>
                player.Hand.Any(card => card.Value is TwoOfClubs));
            if (iTrickStartPlayer == -1)
                throw new InvalidOperationException(
                    $"Could not find a player with the {nameof(TwoOfClubs)}");

            bool isHeartsBroken = false;
            (iTrickStartPlayer, isHeartsBroken) =
                await PlayOutTrick(isFirstTrick: true, iTrickStartPlayer, isHeartsBroken, cancellationToken);

            while (_players[0].Hand.Any())
            {
                (iTrickStartPlayer, isHeartsBroken) =
                    await PlayOutTrick(isFirstTrick: false, iTrickStartPlayer, isHeartsBroken, cancellationToken);
            }

            if (_players.Any(player => player.Hand.Any()))
                throw new InvalidOperationException("Some players have cards left despite the 0th player having none");

            ScoreTricks();
        }

        LogWinnersAndLosers();

        _logger.LogInformation("Completed the game of hearts");
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
            _players[i].Hand = hands[i];

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
                validateChosenCards: (_, iCardsToPlay) => iCardsToPlay.Count == 3,
                cancellationToken);
            takeCardsFromPlayerTasks.Add(task);
        }

        await Task.WhenAll(takeCardsFromPlayerTasks).WaitAsync(cancellationToken);

        _logger.LogInformation("Passing cards");
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
            _players[iTargetPlayer].Hand.AddRange(cardsToPass);
        }

        _logger.LogInformation("Hands are finalized");
    }

    private async Task<(int iNextTrickStartPlayer, bool isHeartsBroken)> PlayOutTrick(
        bool isFirstTrick,
        int iTrickStartPlayer,
        bool isHeartsBroken,
        CancellationToken cancellationToken)
    {
        CircularCounter iTrickPlayer = new(seed: iTrickStartPlayer, maxExclusive: NumPlayers);
        while (true)
        {
            _logger.LogInformation("Getting trick's opening card from player {Name} (position {PlayerPosition})", _players[iTrickPlayer.N].Name,
                iTrickPlayer.N);
            if (!isHeartsBroken && _players[iTrickStartPlayer].Hand.All(card => card.Value.Suit is Suit.Hearts))
            {
                _logger.LogInformation(
                    "Hearts has not been broken and player {Name} (position {PlayerPosition}) only has hearts, skipping to the next player",
                    _players[iTrickPlayer.N].Name, iTrickPlayer.N);
                iTrickPlayer.CycleClockwise();
            }
            else
                break;
        }

        HeartsCard openingCard = await _players[iTrickStartPlayer].PlayCard(
            validateChosenCard: (hand, iCardToPlay) => isFirstTrick
                ? hand[iCardToPlay].Value is TwoOfClubs
                : isHeartsBroken || hand[iCardToPlay].Value.Suit is not Suit.Hearts,
            cancellationToken);
        _logger.LogInformation("Player {Name} (position {PlayerPosition}) played {CardValue}", _players[iTrickPlayer.N].Name, iTrickPlayer.N,
            openingCard.Value);
        Cards<HeartsCard> trick = new(capacity: NumPlayers) { openingCard };
        Suit suitToFollow = openingCard.Value.Suit;

        while (iTrickPlayer.CycleClockwise() != iTrickStartPlayer)
        {
            _logger.LogInformation("Getting trick's next card from player {Name} (position {PlayerPosition})", _players[iTrickPlayer.N].Name,
                iTrickPlayer.N);
            HeartsCard chosenCard = await _players[iTrickPlayer.N].PlayCard(
                validateChosenCard: (hand, iCardToPlay) =>
                {
                    if (!CheckPlayedCard.IsSuitFollowedIfPossible(suitToFollow, hand, iCardToPlay))
                        return false;
                    if (isFirstTrick && hand[iCardToPlay].Points != 0)
                        return false;
                    return true;
                },
                cancellationToken);
            trick.Add(chosenCard);
            _logger.LogInformation("Player {Name} (position {PlayerPosition}) played {CardValue}", _players[iTrickPlayer.N].Name, iTrickPlayer.N,
                chosenCard.Value);

            if (!isHeartsBroken && chosenCard.Value.Suit is Suit.Hearts)
            {
                _logger.LogInformation("Hearts has been broken!");
                isHeartsBroken = true;
            }
        }

        if (trick.Count != NumPlayers)
            throw new InvalidOperationException($"After playing a trick, the trick has {trick.Count} cards but expected {NumPlayers} cards");

        IEnumerable<HeartsCard> onSuitCards = trick.Where(card => card.Value.Suit == suitToFollow);
        Rank highestOnSuitRank = GetHighest.Of(HeartsRankPriorities.Value, onSuitCards.Select(card => card.Value.Rank).ToList());
        int iTrickTakerOffsetFromStartPlayer = trick.FindIndex(card => card.Value.Suit == suitToFollow && card.Value.Rank == highestOnSuitRank);
        if (iTrickTakerOffsetFromStartPlayer == -1)
            throw new InvalidOperationException($"Could not find a card in the trick with suit {suitToFollow} and rank {highestOnSuitRank}");
        int iNextTrickStartPlayer = new CircularCounter(seed: iTrickStartPlayer, maxExclusive: NumPlayers)
            .Tick(delta: iTrickTakerOffsetFromStartPlayer);
        _logger.LogInformation("Player {Name} (position {PlayerPosition}) took the trick with {Card}", _players[iNextTrickStartPlayer].Name,
            iNextTrickStartPlayer, trick[iTrickTakerOffsetFromStartPlayer]);
        _players[iNextTrickStartPlayer].TricksTakenThisRound.Add(trick);

        return (iNextTrickStartPlayer, isHeartsBroken);
    }

    private void ScoreTricks()
    {
        _logger.LogInformation("Scoring tricks");
        List<int> roundScores = new(capacity: NumPlayers);
        foreach (HeartsPlayer heartsPlayer in _players)
        {
            int roundScore = heartsPlayer.TricksTakenThisRound.Sum(trickCards => trickCards.Sum(card => card.Points));
            roundScores.Add(roundScore);
        }

        if (roundScores.Count(score => score == 0) == 3)
        {
            int iPlayerShotTheMoon = roundScores.FindIndex(score => score != 0);
            _logger.LogInformation("Player {Name} (position {PlayerPosition}) shot the moon!", _players[iPlayerShotTheMoon].Name, iPlayerShotTheMoon);
            int score = roundScores[iPlayerShotTheMoon];
            for (int i = 0; i < roundScores.Count; i++)
            {
                if (i != iPlayerShotTheMoon)
                    roundScores[i] = score;
            }

            return;
        }

        for (int i = 0; i < roundScores.Count; i++)
        {
            _players[i].Score += roundScores[i];
            _logger.LogInformation(
                "Player {Name} (position {PlayerPosition}) scored {RoundPoints} point(s) this round and has a total of {TotalPoints} point(s)", i,
                _players[i].Name, roundScores[i], _players[i].Score);
        }
    }

    private void LogWinnersAndLosers()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            HeartsPlayer player = _players[i];
            if (player.Score < _options.EndOfGamePoints)
                continue;
            _logger.LogInformation("Player {Name} (position {PlayerPosition}) is at or over {EndOfGamePoints} points with {TotalPoints}",
                player.Name, i, _options.EndOfGamePoints, player.Score);
        }

        int minScore = _players.Min(player => player.Score);
        for (int i = 0; i < _players.Count; i++)
        {
            HeartsPlayer player = _players[i];
            if (player.Score != minScore)
                continue;
            _logger.LogInformation("Player {Name} (position {PlayerPosition}) is the winner with {TotalPoints}!", player.Name, i, player.Score);
        }
    }

    public sealed class Options
    {
        /// <summary>
        /// The game will end when a round is completed and someone has at least this many points.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int EndOfGamePoints { get; set; } = 100;
    }
}
