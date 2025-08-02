using CardGamesPrototype.Lib;

namespace CardGamesPrototype.UnitTests;

public class DeckMock(IEnumerable<Card>? seed = null) : Deck(seed)
{
    public Func<int, int, int>? RngGetInt32Mock { get; set; }
    protected override int RngGetInt32(int fromInclusive, int toExclusive)
    {
        return RngGetInt32Mock is not null
            ? RngGetInt32Mock(fromInclusive, toExclusive)
            : base.RngGetInt32(fromInclusive, toExclusive);
    }
}
