namespace Yahtzee;

public static class StringHelper
{
    public static string PadCenter(this string str, int length, char padChar = ' ')
    {
        var strLength = str.Length;
        if (strLength > length)
        {
            str = str[..length];
            strLength = length;
        }

        var numPadChars = length - strLength;
        var padLeft = length - numPadChars / 2;
        return str.PadLeft(padLeft, padChar).PadRight(length);
    }
}