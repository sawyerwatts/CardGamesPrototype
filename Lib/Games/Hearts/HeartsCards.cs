using CardGamesPrototype.Lib.Common;

namespace CardGamesPrototype.Lib.Games.Hearts;

// TODO: how could I impl HeartsCard, HeartsCards, and HeartsPlayer w/o "inheritance"?

public sealed record HeartsCard : Card
{
    public int Points { get; }

    public HeartsCard(CardValue cardValue)
        : base(cardValue)
    {
        if (cardValue.Suit is Suit.Hearts)
            Points = 1;
        else if (cardValue.Rank is Rank.Queen && cardValue.Suit is Suit.Spades)
            Points = 13;
        else
            Points = 0;
    }

    public new static Cards<HeartsCard> MakeDeck(IEnumerable<CardValue> seed) =>
         new(seed.Select(cardValue => new HeartsCard(cardValue)));
}
