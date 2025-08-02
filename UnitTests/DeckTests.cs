using CardGamesPrototype.Lib;

namespace CardGamesPrototype.UnitTests;

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
        Assert.Equal(FourOfHearts.Instance, deck[0].Card);
        Assert.Equal(FiveOfHearts.Instance, deck[1].Card);
        Assert.Equal(SixOfHearts.Instance, deck[2].Card);
        Assert.Equal(SevenOfHearts.Instance, deck[3].Card);
        Assert.Equal(EightOfHearts.Instance, deck[4].Card);
        Assert.Equal(NineOfHearts.Instance, deck[5].Card);
        Assert.Equal(Joker0.Instance, deck[6].Card);
        Assert.Equal(AceOfHearts.Instance, deck[7].Card);
        Assert.Equal(TwoOfHearts.Instance, deck[8].Card);
        Assert.Equal(ThreeOfHearts.Instance, deck[9].Card);
    }
}
