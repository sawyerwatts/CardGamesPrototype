using CardGamesPrototype.Lib.Common;

namespace CardGamesPrototype.UnitTests;

public class CardsTests
{
    [Fact]
    public void TestMatchesWhenEqual()
    {
        Cards deck = new(
        [
            Joker0.Instance,
            AceOfSpades.Instance,
            NineOfHearts.Instance,
        ]);

        Cards unexpectedDeck = new(
        [
            Joker0.Instance,
            AceOfSpades.Instance,
            NineOfHearts.Instance,
        ]);

        Assert.True(deck.Matches(unexpectedDeck));
    }

    [Fact]
    public void TestMatchesWhenNotEqual()
    {
        Cards deck = new(
        [
            Joker0.Instance,
            AceOfSpades.Instance,
            NineOfHearts.Instance,
        ]);

        Cards unexpectedDeck = new(
        [
            NineOfHearts.Instance,
            AceOfSpades.Instance,
            Joker0.Instance,
        ]);

        Assert.False(deck.Matches(unexpectedDeck));
    }
}
