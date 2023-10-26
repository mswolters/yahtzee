namespace Yahtzee;

public static class Contains
{
    public static bool ContainsAll<T>(this IEnumerable<T> self, IEnumerable<T> other) where T : notnull
    {
        var selfCounts = self.GroupBy(t => t).ToDictionary(g => g.Key, g => g.Count());
        var otherCounts = other.GroupBy(t => t).ToDictionary(g => g.Key, g => g.Count());
        return selfCounts.All(keyCount => otherCounts.GetValueOrDefault(keyCount.Key) <= keyCount.Value);
    }
}