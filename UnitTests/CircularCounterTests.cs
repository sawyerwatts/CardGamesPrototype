using CardGamesPrototype.Lib;

namespace CardGamesPrototype.UnitTests;

public class CircularCounterTests
{
    [Fact]
    public void TestCycling()
    {
        CircularCounter sut = new(3);
        Assert.Equal(0, sut.N);

        Assert.Equal(1, sut.Increment());
        Assert.Equal(1, sut.N);

        Assert.Equal(2, sut.Increment());
        Assert.Equal(2, sut.N);

        Assert.Equal(0, sut.Increment());
        Assert.Equal(0, sut.N);
    }

    [Fact]
    public void TestMultiIncrementsThatDontRollOver()
    {
        CircularCounter sut = new(3);
        Assert.Equal(0, sut.N);

        Assert.Equal(2, sut.Increment(2));
        Assert.Equal(2, sut.N);

        Assert.Equal(0, sut.Increment());
        Assert.Equal(0, sut.N);
    }

    [Fact]
    public void TestMultiIncrementsThatRollOver()
    {
        CircularCounter sut = new(3);
        Assert.Equal(0, sut.N);

        Assert.Equal(1, sut.Increment(4));
        Assert.Equal(1, sut.N);
    }
}
