using System.Text;

namespace CardGamesPrototype.Lib.Common;

public class Cards : List<Card>
{
    public Cards(int capacity = 0) : base(capacity) { }

    public Cards(IEnumerable<Card> seed) : base(seed) { }

    public void FlipFaceUp()
    {
        foreach (Card card in this)
            card.IsFaceUp = true;
    }

    public void FlipFaceDown()
    {
        foreach (Card card in this)
            card.IsFaceUp = true;
    }

    public override string ToString()
    {
        StringBuilder builder = new();
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

    // TODO: should this sort b/f checking? or at least have a param to ctl?
    public bool Matches(Cards other)
    {
        if (ReferenceEquals(this, other))
            return true;
        if (Count != other.Count)
            return false;
        for (int i = 0; i < this.Count; i++)
        {
            Card thisCard = this[i];
            Card otherCard = other[i];
            if (thisCard != otherCard)
                return false;
        }

        return true;
    }
}
public record Card(CardValue CardValue)
{
    public bool IsFaceUp { get; set; } = false;
}

public abstract record CardValue(Rank Rank, Suit Suit)
{
    public override string ToString()
    {
        return Suit is Suit.Joker
            ? Rank.ToString()
            : $"{Rank} of {Suit}";
    }
}

public enum Rank
{
    Ace,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Joker0,
    Joker1,
}

public enum Suit
{
    Hearts,
    Spades,
    Diamonds,
    Clubs,
    Joker,
}
