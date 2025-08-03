using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace CardGamesPrototype.Lib.Common;

/// <remarks>
/// It is intended that the deck's index counts upwards, so deck[0] is the bottom card of the deck.
/// </remarks>
/// <param name="seed"></param>
public partial class Deck(IEnumerable<Card>? seed = null) : IList<Deck.CardState>
{
    private List<CardState> _cards = new(seed?.Select(card => new CardState(card)) ?? []);

    public sealed record CardState(Card Card)
    {
        public bool IsFaceUp { get; set; } = false;
    }

    public sealed record Specification
    {
        public static readonly Specification Standard52CardDeck = new();

        public bool IncludeJokers { get; init; } = false;
    }

    /// <summary>
    /// This makes a deck per the supplied <paramref name="spec"/> (or the default if not supplied). This will not
    /// <see cref="Shuffle"/> or <see cref="Cut"/> the deck.
    /// </summary>
    /// <param name="spec"></param>
    /// <returns></returns>
    public static Deck Make(Specification? spec = null)
    {
        spec ??= Specification.Standard52CardDeck;
        var deck = new Deck()
        {
            new CardState(AceOfHearts.Instance),
            new CardState(TwoOfHearts.Instance),
            new CardState(ThreeOfHearts.Instance),
            new CardState(FourOfHearts.Instance),
            new CardState(FiveOfHearts.Instance),
            new CardState(SixOfHearts.Instance),
            new CardState(SevenOfHearts.Instance),
            new CardState(EightOfHearts.Instance),
            new CardState(NineOfHearts.Instance),
            new CardState(TenOfHearts.Instance),
            new CardState(JackOfHearts.Instance),
            new CardState(QueenOfHearts.Instance),
            new CardState(KingOfHearts.Instance),

            new CardState(AceOfSpades.Instance),
            new CardState(TwoOfSpades.Instance),
            new CardState(ThreeOfSpades.Instance),
            new CardState(FourOfSpades.Instance),
            new CardState(FiveOfSpades.Instance),
            new CardState(SixOfSpades.Instance),
            new CardState(SevenOfSpades.Instance),
            new CardState(EightOfSpades.Instance),
            new CardState(NineOfSpades.Instance),
            new CardState(TenOfSpades.Instance),
            new CardState(JackOfSpades.Instance),
            new CardState(QueenOfSpades.Instance),
            new CardState(KingOfSpades.Instance),

            new CardState(AceOfDiamonds.Instance),
            new CardState(TwoOfDiamonds.Instance),
            new CardState(ThreeOfDiamonds.Instance),
            new CardState(FourOfDiamonds.Instance),
            new CardState(FiveOfDiamonds.Instance),
            new CardState(SixOfDiamonds.Instance),
            new CardState(SevenOfDiamonds.Instance),
            new CardState(EightOfDiamonds.Instance),
            new CardState(NineOfDiamonds.Instance),
            new CardState(TenOfDiamonds.Instance),
            new CardState(JackOfDiamonds.Instance),
            new CardState(QueenOfDiamonds.Instance),
            new CardState(KingOfDiamonds.Instance),

            new CardState(AceOfClubs.Instance),
            new CardState(TwoOfClubs.Instance),
            new CardState(ThreeOfClubs.Instance),
            new CardState(FourOfClubs.Instance),
            new CardState(FiveOfClubs.Instance),
            new CardState(SixOfClubs.Instance),
            new CardState(SevenOfClubs.Instance),
            new CardState(EightOfClubs.Instance),
            new CardState(NineOfClubs.Instance),
            new CardState(TenOfClubs.Instance),
            new CardState(JackOfClubs.Instance),
            new CardState(QueenOfClubs.Instance),
            new CardState(KingOfClubs.Instance),
        };

        if (spec.IncludeJokers)
        {
            deck.Add(new CardState(Joker0.Instance));
            deck.Add(new CardState(Joker1.Instance));
        }

        return deck;
    }

    /// <summary>
    /// This will perform an in-place shuffle of the <see cref="Deck"/> instance.
    /// </summary>
    /// <returns></returns>
    public virtual Deck Shuffle()
    {
        RandomNumberGenerator.Shuffle(CollectionsMarshal.AsSpan(_cards));
        return this;
    }

    /// <summary>
    /// This will perform an in-place cut of the <see cref="Deck"/> instance.
    /// </summary>
    /// <returns></returns>
    public virtual Deck Cut(int minNumCardsFromEdges = 1)
    {
        if (minNumCardsFromEdges < 1)
            throw new ArgumentException($"{nameof(minNumCardsFromEdges)} must be positive, but was given {minNumCardsFromEdges}");
        if (this.Count < 2)
            return this;

        int newTopCardIndex;
        try
        {
            newTopCardIndex = RngGetInt32(
                fromInclusive: minNumCardsFromEdges,
                toExclusive: this.Count - minNumCardsFromEdges);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new ArgumentException($"The deck is too small to cut while not cutting the top or bottom {minNumCardsFromEdges}");
        }

        IEnumerable<CardState> cardsBelowAndAtCut = _cards.Take(newTopCardIndex+1);
        IEnumerable<CardState> cardsAboveCut = _cards.Skip(newTopCardIndex+1);
        List<CardState> newCards = new(capacity: _cards.Count);
        newCards.AddRange(cardsAboveCut);
        newCards.AddRange(cardsBelowAndAtCut);
        _cards = newCards;
        return this;
    }

    /// <inheritdoc cref="RandomNumberGenerator.GetInt32(int,int)"/>
    protected virtual int RngGetInt32(int fromInclusive, int toExclusive)
    {
        return RandomNumberGenerator.GetInt32(
            fromInclusive: fromInclusive,
            toExclusive: toExclusive);
    }

    public List<Deck> Deal(int numHands)
    {
        if (numHands < 1)
            throw new ArgumentException($"{nameof(numHands)} must be positive but given {numHands}");

        List<Deck> hands = new(capacity: numHands);
        for (int i = 0; i < numHands; i++)
            hands.Add([]);

        CircularCounter iCurrHand = new(numHands);
        foreach (CardState currCard in this)
        {
            hands[iCurrHand.N].Add(currCard);
            iCurrHand.Tick();
        }

        return hands;
    }

    public List<Card> GetCards()
    {
        List<Card> cards = new(capacity: this.Count);
        cards.AddRange(this.Select(cardState => cardState.Card));
        return cards;
    }

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

    public bool Matches(Deck other)
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
}
