namespace CardGamesPrototype.Lib;

public abstract record Card(Rank Rank, Suit Suit)
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
