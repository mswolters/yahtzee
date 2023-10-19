namespace Yahtzee.Tests;

internal class YahtzeeAssert
{

    public static void AreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, string message = "", params object[] parameters)
    {
        if (!expected.SequenceEqual(actual))
        {
            Assert.Fail($"{message} Enumerables not equal\nExpected: {Stringify(expected)}\nActual: {Stringify(actual)}");
        }
    }

    private static string Stringify<T>(IEnumerable<T> e)
    {
        return $"[{string.Join(',', e)}]";
    }

}