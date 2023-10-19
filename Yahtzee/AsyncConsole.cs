internal class AsyncConsole {
    public static async Task<string> ReadLineAsync()
    {
        var result = await Task.Run(Console.ReadLine);
        return result ?? string.Empty;
    }
}