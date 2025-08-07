using CardGamesPrototype.Lib.Common;

using Microsoft.Extensions.Logging.Abstractions;

namespace CardGamesPrototype.UnitTests.Common;

public sealed class DealerTests
{
    private readonly Dealer _dealer;
    private readonly RngMock _rng;

    public DealerTests()
    {
        _rng = new RngMock();
        _dealer = new Dealer(_rng, NullLogger<Dealer>.Instance);
    }

    [Fact]
    public void TestCut()
    {
        Cards deck = new(
        [
            Joker0.Instance,
            AceOfHearts.Instance,
            TwoOfHearts.Instance,
            ThreeOfHearts.Instance,
            FourOfHearts.Instance,
            FiveOfHearts.Instance,
            SixOfHearts.Instance,
            SevenOfHearts.Instance,
            EightOfHearts.Instance,
            NineOfHearts.Instance,
        ]);

        _rng.GetInt32Value = 3;
        deck = _dealer.Cut(deck);

        Cards expectedDeck = new(
        [
            FourOfHearts.Instance,
            FiveOfHearts.Instance,
            SixOfHearts.Instance,
            SevenOfHearts.Instance,
            EightOfHearts.Instance,
            NineOfHearts.Instance,
            Joker0.Instance,
            AceOfHearts.Instance,
            TwoOfHearts.Instance,
            ThreeOfHearts.Instance,
        ]);
        Assert.True(deck.Matches(expectedDeck));
    }

    [Fact]
    public void TestShuffle()
    {
        Cards deck = Decks.Standard52();
        Cards prevDeck = new(deck);

        int numChangedDecks = 0;
        for (int i = 0; i < 5; i++)
        {
            deck = _dealer.Shuffle(deck);
            if (!deck.Matches(prevDeck))
                numChangedDecks++;
            prevDeck = new Cards(deck);
        }

        Assert.True(numChangedDecks > 3);
    }

    [Fact]
    public void TestDeal()
    {
        Cards deck = new(
        [
            Joker0.Instance,
            AceOfHearts.Instance,
            TwoOfHearts.Instance,
            ThreeOfHearts.Instance,
            FourOfHearts.Instance,
            FiveOfHearts.Instance,
            SixOfHearts.Instance,
            SevenOfHearts.Instance,
            EightOfHearts.Instance,
            NineOfHearts.Instance,
        ]);

        Cards expectedHand0 = new(
        [
            Joker0.Instance,
            FourOfHearts.Instance,
            EightOfHearts.Instance,
        ]);

        Cards expectedHand1 = new(
        [
            AceOfHearts.Instance,
            FiveOfHearts.Instance,
            NineOfHearts.Instance,
        ]);

        Cards expectedHand2 = new(
        [
            TwoOfHearts.Instance,
            SixOfHearts.Instance,
        ]);

        Cards expectedHand3 = new(
        [
            ThreeOfHearts.Instance,
            SevenOfHearts.Instance,
        ]);

        List<Cards> hands = _dealer.Deal(deck, 4);
        Assert.Equal(4, hands.Count);
        Assert.True(expectedHand0.Matches(hands[0]));
        Assert.True(expectedHand1.Matches(hands[1]));
        Assert.True(expectedHand2.Matches(hands[2]));
        Assert.True(expectedHand3.Matches(hands[3]));
    }

    private class RngMock : Dealer.IRng
    {
        private readonly Dealer.Rng _rng = new();

        public void Shuffle<T>(Span<T> values) => _rng.Shuffle(values);

        public int? GetInt32Value { get; set; }

        public int GetInt32(int fromInclusive, int toExclusive) =>
            GetInt32Value ?? _rng.GetInt32(fromInclusive, toExclusive);
    }
}
