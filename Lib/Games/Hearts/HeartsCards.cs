using CardGamesPrototype.Lib.Common;

namespace CardGamesPrototype.Lib.Games.Hearts;

public sealed record HeartsCard : Card
{
    public int Points { get; }

    public HeartsCard(Card card, int points)
        : base(card)
    {
        Points = points;
    }
}

public sealed class HeartsCards : Cards
{
    public HeartsCards(int capacity = 0) : base(capacity) { }

    public HeartsCards(IEnumerable<Card> seed) : base(seed) { }

    public HeartsCards(IEnumerable<CardValue> seed) : base(seed.Select(cardValue =>
        new Card(cardValue)))
    {
    }

    public static HeartsCards MakeDeck()
    {
        Cards normalDeck = Decks.Standard52();
        HeartsCards heartsDeck = new(capacity: normalDeck.Count);
        foreach (Card card in normalDeck)
        {
            int points = 0;
            if (card.CardValue.Suit is Suit.Hearts)
                points = 0;
            else if (card.CardValue.Rank is Rank.Queen && card.CardValue.Suit is Suit.Spades)
                points = 13;
            HeartsCard heartsCard = new(card, points);
            heartsDeck.Add(heartsCard);
        }

        return heartsDeck;
    }
}

