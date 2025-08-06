using System.Runtime.InteropServices;
using System.Security.Cryptography;

using Microsoft.Extensions.Logging;

namespace CardGamesPrototype.Lib.Common;

public interface IDealer
{
    /// <remarks>
    /// This may or may not mutate the supplied <paramref name="deck"/>.
    /// </remarks>
    List<Cards> ShuffleCutDeal(Cards deck, int numHands);

    /// <remarks>
    /// This may or may not mutate the supplied <paramref name="deck"/>.
    /// </remarks>
    Cards Shuffle(Cards deck);

    /// <remarks>
    /// This may or may not mutate the supplied <paramref name="deck"/>.
    /// </remarks>
    Cards Cut(Cards deck, int minNumCardsFromEdges = 1);

    List<Cards> Deal(Cards deck, int numHands);
}

public class Dealer(ILogger<Dealer> logger) : IDealer
{
    public List<Cards> ShuffleCutDeal(Cards deck, int numHands)
    {
        Cards shuffled = Shuffle(deck);
        Cards cut = Cut(shuffled);
        return Deal(cut, numHands);
    }

    public Cards Shuffle(Cards deck)
    {
        logger.LogInformation("Shuffling the deck");
        RandomNumberGenerator.Shuffle(CollectionsMarshal.AsSpan(deck));
        return deck;
    }

    public Cards Cut(Cards deck, int minNumCardsFromEdges = 1)
    {
        logger.LogInformation(
            "Cutting the deck with the cut occurring at least {MinNumCardsFromEdges} cards from the top and bottom",
            minNumCardsFromEdges);
        if (minNumCardsFromEdges < 1)
            throw new ArgumentException(
                $"{nameof(minNumCardsFromEdges)} must be positive, but was given {minNumCardsFromEdges}");
        if (deck.Count < 2)
            return deck;

        int newTopCardIndex;
        try
        {
            newTopCardIndex = RandomNumberGenerator.GetInt32(
                fromInclusive: minNumCardsFromEdges,
                toExclusive: deck.Count - minNumCardsFromEdges);
        }
        catch (ArgumentOutOfRangeException)
        {
            logger.LogError("The deck is too small to cut while not cutting the top or bottom {MinNumCardsFromEdges}", minNumCardsFromEdges);
            throw new ArgumentException(
                $"The deck is too small to cut while not cutting the top or bottom {minNumCardsFromEdges}");
        }

        IEnumerable<Card> cardsBelowAndAtCut = deck.Take(newTopCardIndex + 1);
        IEnumerable<Card> cardsAboveCut = deck.Skip(newTopCardIndex + 1);
        Cards newCards = new(capacity: deck.Count);
        newCards.AddRange(cardsAboveCut);
        newCards.AddRange(cardsBelowAndAtCut);
        return newCards;
    }

    public List<Cards> Deal(Cards deck, int numHands)
    {
        logger.LogInformation("Dealing the deck to {NumHands} hands", numHands);
        if (numHands < 1)
            throw new ArgumentException(
                $"{nameof(numHands)} must be positive but given {numHands}");

        List<Cards> hands = new(capacity: numHands);
        for (int i = 0; i < numHands; i++)
            hands.Add([]);

        CircularCounter iCurrHand = new(numHands);
        foreach (Card currCard in deck)
        {
            hands[iCurrHand.N].Add(currCard);
            iCurrHand.Tick();
        }

        return hands;
    }
}
