using CardGamesPrototype.Lib.Common;

namespace CardGamesPrototype.Lib.Games.Hearts;

// TODO: how could I impl HeartsCard, HeartsCards, and HeartsPlayer w/o "inheritance"?

// TODO: need IDealer methods w/ <TCard> where TCard : Card
// TODO: need Player methods w/ <TCard> where TCard : Card ?

public sealed record HeartsCard : Card
{
    public int Points { get; }

    public HeartsCard(Card card)
        : base(card)
    {
        if (card.CardValue.Suit is Suit.Hearts)
            Points = 1;
        else if (card.CardValue.Rank is Rank.Queen && card.CardValue.Suit is Suit.Spades)
            Points = 13;
        else
            Points = 0;
    }
}

public sealed class HeartsCards : Cards
{
    public HeartsCards(int capacity = 0) : base(capacity) { }

    public HeartsCards(IEnumerable<HeartsCard> seed) : base(seed) { }

    public HeartsCards(IEnumerable<Card> seed) : base(seed.Select(card => new HeartsCard(card))) { }
}

