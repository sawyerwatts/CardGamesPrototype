using CardGamesPrototype.Lib.Common;

namespace CardGamesPrototype.UnitTests.Common;

public class DecksTests
{
    [Fact]
    public void TestStandard52Has52UniqueCards()
    {
        Cards deck = Decks.Standard52();
        Assert.Equal(52, deck.Count);
        Assert.Equal(52, deck.Distinct().Count());
    }
}
