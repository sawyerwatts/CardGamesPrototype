using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace CardGamesPrototype.Lib;

// TODO: a future enhancement, like for Chimera, would be to store if the card is face up or face down
//      could prob also have a flip method to reverse the items in the list and change the face up bool
//      this would be a fun one, esp since it has non-standard shuffling/dealing/betting
//      have a deal func that knows how many hands to deal to, and the optional max capacity of those hands? and/or a priority of the hands to deal to?

/// <remarks>
/// It is intended that the deck's index counts upwards, so deck[0] is the bottom card of the deck.
/// </remarks>
/// <param name="seed"></param>
public partial class Deck(IEnumerable<Card>? seed = null) : IList<Card>
{
    private List<Card> _cards = new List<Card>(seed ?? []);

    public sealed record Specification
    {
        public static readonly Specification Standard52CardDeck = new();

        public bool IncludeJokers { get; init; } = false;
    }

    public static Deck MakeShuffleCut(Specification? spec = null)
    {
        var deck = Make(spec);
        deck.Shuffle();
        deck.Cut();
        return deck;
    }

    public static Deck Make(Specification? spec = null)
    {
        spec ??= Specification.Standard52CardDeck;
        var deck = new Deck()
        {
            AceOfHearts.Instance,
            TwoOfHearts.Instance,
            ThreeOfHearts.Instance,
            FourOfHearts.Instance,
            FiveOfHearts.Instance,
            SixOfHearts.Instance,
            SevenOfHearts.Instance,
            EightOfHearts.Instance,
            NineOfHearts.Instance,
            TenOfHearts.Instance,
            JackOfHearts.Instance,
            QueenOfHearts.Instance,
            KingOfHearts.Instance,

            AceOfSpades.Instance,
            TwoOfSpades.Instance,
            ThreeOfSpades.Instance,
            FourOfSpades.Instance,
            FiveOfSpades.Instance,
            SixOfSpades.Instance,
            SevenOfSpades.Instance,
            EightOfSpades.Instance,
            NineOfSpades.Instance,
            TenOfSpades.Instance,
            JackOfSpades.Instance,
            QueenOfSpades.Instance,
            KingOfSpades.Instance,

            AceOfDiamonds.Instance,
            TwoOfDiamonds.Instance,
            ThreeOfDiamonds.Instance,
            FourOfDiamonds.Instance,
            FiveOfDiamonds.Instance,
            SixOfDiamonds.Instance,
            SevenOfDiamonds.Instance,
            EightOfDiamonds.Instance,
            NineOfDiamonds.Instance,
            TenOfDiamonds.Instance,
            JackOfDiamonds.Instance,
            QueenOfDiamonds.Instance,
            KingOfDiamonds.Instance,

            AceOfClubs.Instance,
            TwoOfClubs.Instance,
            ThreeOfClubs.Instance,
            FourOfClubs.Instance,
            FiveOfClubs.Instance,
            SixOfClubs.Instance,
            SevenOfClubs.Instance,
            EightOfClubs.Instance,
            NineOfClubs.Instance,
            TenOfClubs.Instance,
            JackOfClubs.Instance,
            QueenOfClubs.Instance,
            KingOfClubs.Instance,
        };

        if (spec.IncludeJokers)
        {
            deck.Add(Joker0.Instance);
            deck.Add(Joker1.Instance);
        }

        return deck;
    }

    public virtual void Shuffle()
    {
        RandomNumberGenerator.Shuffle(CollectionsMarshal.AsSpan(_cards));
    }

    public virtual void Cut(int minNumCardsFromEdges = 1)
    {
        if (minNumCardsFromEdges < 1)
            throw new ArgumentException($"{nameof(minNumCardsFromEdges)} must be positive, but was given {minNumCardsFromEdges}");
        if (this.Count < 2)
            return;

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

        IEnumerable<Card> belowAndAtCards = _cards.Take(newTopCardIndex+1);
        IEnumerable<Card> aboveCards = _cards.Skip(newTopCardIndex+1);
        List<Card> newCards = new(capacity: _cards.Capacity);
        newCards.AddRange(aboveCards);
        newCards.AddRange(belowAndAtCards);
        _cards = newCards;
    }

    /// <inheritdoc cref="RandomNumberGenerator.GetInt32(int,int)"/>
    protected virtual int RngGetInt32(int fromInclusive, int toExclusive)
    {
        return RandomNumberGenerator.GetInt32(
            fromInclusive: fromInclusive,
            toExclusive: toExclusive);
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
}
