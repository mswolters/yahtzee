namespace Yahtzee.Tests.Mock;

internal class MockRandom : Random
{
    private List<int> _mockedInts = new();
    private List<float> _mockedFloats = new();

    public MockRandom(int seed = 0) : base(seed) { }

    public void MockNext(params int[] MockedInts)
    {
        _mockedInts = new List<int>(MockedInts);
    }

    public void MockNext(params float[] MockedFloats)
    {
        _mockedFloats = new List<float>(MockedFloats);
    }

    public override int Next()
    {
        return _mockedInts.RemoveFirstOrDefault(() => base.Next());
    }

    public override int Next(int maxValue)
    {
        return _mockedInts.RemoveFirstOrDefault(() => base.Next(maxValue));
    }

    public override int Next(int minValue, int maxValue)
    {
        return _mockedInts.RemoveFirstOrDefault(() => base.Next(minValue, maxValue));
    }

    public override float NextSingle()
    {
        return _mockedFloats.RemoveFirstOrDefault(() => base.NextSingle());
    }

}

internal static class ListExtension
{
    public static T RemoveFirstOrDefault<T>(this List<T> list, Func<T> defaultGenerator)
    {
        if (list.Any())
        {
            T first = list.First();
            list.RemoveAt(0);
            return first;
        }
        return defaultGenerator();
    }
}