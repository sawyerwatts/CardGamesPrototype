using CardGamesPrototype.Lib;

namespace CardGamesPrototype.UnitTests;

// TODO: make a Dealer for the deck so can more conventionally inject/mock rng?

public class DeckTests
{
    [Fact]
    public void TestMakeWithDefaults()
    {
        var deck = Deck.Make(Deck.Specification.Standard52CardDeck);
        Assert.Equal(52, deck.Count);
        Assert.Equal(52, deck.Distinct().Count());
    }

    [Fact]
    public void TestCut()
    {
        var deck = new DeckMock(
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
        deck.RngGetInt32Mock = (_, _) => 3;

        deck.Cut();

        Assert.Equal(10, deck.Count);
        Assert.Equal(FourOfHearts.Instance, deck[0]);
        Assert.Equal(FiveOfHearts.Instance, deck[1]);
        Assert.Equal(SixOfHearts.Instance, deck[2]);
        Assert.Equal(SevenOfHearts.Instance, deck[3]);
        Assert.Equal(EightOfHearts.Instance, deck[4]);
        Assert.Equal(NineOfHearts.Instance, deck[5]);
        Assert.Equal(Joker0.Instance, deck[6]);
        Assert.Equal(AceOfHearts.Instance, deck[7]);
        Assert.Equal(TwoOfHearts.Instance, deck[8]);
        Assert.Equal(ThreeOfHearts.Instance, deck[9]);
    }
}
