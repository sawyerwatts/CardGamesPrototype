using CardGamesPrototype.Lib.Shared;

namespace CardGamesPrototype.UnitTests.Shared;

public class DecksTests
{
    [Fact]
    public void TestStandard52Has52UniqueCards()
    {
        List<CardValue> deck = Decks.Standard52();
        Assert.Equal(52, deck.Count);
        Assert.Equal(52, deck.Distinct().Count());
    }
}
