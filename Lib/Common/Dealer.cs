using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace CardGamesPrototype.Lib.Common;

// TODO: Seed ctor would be nice too
public class Cards(int capacity = 0) : List<CardState>(capacity)
{
    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append('[');
        for (int i = 0; i < this.Count; i++)
        {
            if (i == 0)
                builder.AppendLine();
            builder.Append(i);
            builder.Append(": ");
            builder.Append(this[i]);
            builder.AppendLine();
        }

        builder.Append(']');
        return builder.ToString();
    }

    public bool Matches(Cards other)
    {
        if (ReferenceEquals(this, other))
            return true;
        if (Count != other.Count)
            return false;
        for (int i = 0; i < this.Count; i++)
        {
            CardState thisCardState = this[i];
            CardState otherCardState = other[i];
            if (thisCardState != otherCardState)
                return false;
        }

        return true;
    }
};

public record CardState(Card Card)
{
    public bool IsFaceUp { get; set; } = false;
}

public interface IDealer
{
    /// <remarks>
    /// This may or may not mutate the supplied <paramref name="cards"/>.
    /// </remarks>
    Cards Shuffle(Cards cards);

    /// <remarks>
    /// This may or may not mutate the supplied <paramref name="cards"/>.
    /// </remarks>
    Cards Cut(Cards cards, int minNumCardsFromEdges = 1);
}

// TODO: log too
public class Dealer : IDealer
{
    public Cards Shuffle(Cards cards)
    {
        RandomNumberGenerator.Shuffle(CollectionsMarshal.AsSpan(cards));
        return cards;
    }

    public Cards Cut(Cards cards, int minNumCardsFromEdges = 1)
    {
        if (minNumCardsFromEdges < 1)
            throw new ArgumentException($"{nameof(minNumCardsFromEdges)} must be positive, but was given {minNumCardsFromEdges}");
        if (cards.Count < 2)
            return cards;

        int newTopCardIndex;
        try
        {
            newTopCardIndex = RandomNumberGenerator.GetInt32(
                fromInclusive: minNumCardsFromEdges,
                toExclusive: cards.Count - minNumCardsFromEdges);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new ArgumentException($"The deck is too small to cut while not cutting the top or bottom {minNumCardsFromEdges}");
        }

        IEnumerable<CardState> cardsBelowAndAtCut = cards.Take(newTopCardIndex+1);
        IEnumerable<CardState> cardsAboveCut = cards.Skip(newTopCardIndex+1);
        Cards newCards = new(capacity: cards.Count);
        newCards.AddRange(cardsAboveCut);
        newCards.AddRange(cardsBelowAndAtCut);
        return newCards;
    }

}
