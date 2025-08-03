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

        var expectedDeck = new Deck(
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
        Assert.True(expectedDeck.Matches(deck));
    }

    [Fact]
    public void TestShuffle()
    {
        Deck deck = Deck.Make();
        Deck unexpectedDeck = Deck.Make();

        bool pass = false;
        for (int i = 0; i < 5; i++)
        {
            deck.Shuffle();
            if (!deck.Matches(unexpectedDeck))
            {
                pass = true;
                break;
            }
        }
        Assert.True(pass);
    }

    [Fact]
    public void TestDeal()
    {
        Deck deck = new Deck(
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

        Deck expectedHand0 = new Deck(
        [
            Joker0.Instance,
            FourOfHearts.Instance,
            EightOfHearts.Instance,
        ]);

        Deck expectedHand1 = new Deck(
        [
            AceOfHearts.Instance,
            FiveOfHearts.Instance,
            NineOfHearts.Instance,
        ]);

        Deck expectedHand2 = new Deck(
        [
            TwoOfHearts.Instance,
            SixOfHearts.Instance,
        ]);

        Deck expectedHand3 = new Deck(
        [
            ThreeOfHearts.Instance,
            SevenOfHearts.Instance,
        ]);

        List<Deck> hands = deck.Deal(4);
        Assert.Equal(4, hands.Count);
        Assert.True(expectedHand0.Matches(hands[0]));
        Assert.True(expectedHand1.Matches(hands[1]));
        Assert.True(expectedHand2.Matches(hands[2]));
        Assert.True(expectedHand3.Matches(hands[3]));
    }
}
