using System.Text;

namespace CardGamesPrototype.Lib.Shared;

public class Cards<TCard> : List<TCard>
    where TCard : Card
{
    public Cards(int capacity = 0) : base(capacity) { }

    public Cards(IEnumerable<TCard> seed) : base(seed) { }

    public void RevealAll()
    {
        foreach (TCard card in this)
            card.Hidden = false;
    }

    public void HideAll()
    {
        foreach (TCard card in this)
            card.Hidden = true;
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

    public bool Matches(Cards<TCard> other)
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

public record Card(CardValue Value)
{
    public bool Hidden { get; set; } = true;

    public Card(Card card)
    {
        Value = card.Value;
        Hidden = card.Hidden;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static Cards<Card> MakeDeck(IEnumerable<CardValue> seed) =>
        new(seed.Select(cardValue => new Card(cardValue)));
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
