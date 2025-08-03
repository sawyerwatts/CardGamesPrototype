namespace CardGamesPrototype.Lib;

public sealed class CircularCounter
{
    public int N { get; private set; } = 0;
    private readonly int _maxExclusive;

    public CircularCounter(int maxExclusive)
    {
        if (maxExclusive < 0)
            throw new ArgumentException(
                $"A non-negative {nameof(maxExclusive)} is required but given {maxExclusive}");
        _maxExclusive = maxExclusive;
    }

    public int Increment(int addition = 1)
    {
        if (addition < 0)
            throw new ArgumentException(
                $"{nameof(Increment)} expects a non-negative {nameof(addition)} but given {addition}");
        for (int i = 0; i < addition; i++)
        {
            if (N + 1 == _maxExclusive)
                N = 0;
            else
                N++;
        }

        return N;
    }
}
