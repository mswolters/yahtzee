namespace Yahtzee;

internal static class IndexOf
{

    // It's O(M*N) rather than O(M+N) but it's good enough
    public static IEnumerable<int> IndicesOf<T>(this IList<T> list, IEnumerable<T> itemsToFind) => itemsToFind.Select(list.IndexOf);
    public static IEnumerable<int> IndicesOf<T>(this IList<T> list, params T[] itemsToFind) => list.IndicesOf((IEnumerable<T>)itemsToFind);
    public static IEnumerable<int> IndicesOf<T>(this T[] list, IEnumerable<T> itemsToFind) => itemsToFind.Select(item => Array.IndexOf(list, item));

}