using System.ComponentModel;
using Yahtzee.Models.Rules;

namespace Yahtzee.Models;

internal class ConsolePlayer : IPlayablePlayer
{
    public ConsolePlayer(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task<IList<DieRoll>> PickDiceToKeep(TurnState state, IList<DieRoll> rolls)
    {
        Console.WriteLine($"Current roll: {state.ThrowCount}");
        var keptCount = state.KeptDice.Count;
        if (keptCount > 0)
        {
            Console.WriteLine("Kept dice:");
            foreach (var line in state.KeptDice.Select((roll, index) => $"{index}: {roll.Value}"))
            {
                Console.WriteLine(line);
            }
        }
        Console.WriteLine("New rolls:");
        foreach (var line in rolls.Select((roll, index) => $"{index + keptCount}: {roll.Value}"))
        {
            Console.WriteLine(line);
        }
        var dice = state.KeptDice.Concat(rolls).ToList();
        var success = false;
        IList<DieRoll> selectedDice = new List<DieRoll>();
        while (!success)
        {
            Console.WriteLine("Pick the index of dice to keep with a comma separated list");
            Console.WriteLine("Keep all dice to end your turn early");
            var selectionString = await AsyncConsole.ReadLineAsync();
            var selection = selectionString.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)!;
            var parsedSelection = selection.Select(TryParse).ToList();
            success = AllSuccessAndInRange(dice, parsedSelection);
            if (success)
            {
                selectedDice = parsedSelection.Select(it => dice[it.Value]).ToList();
            }
        }
        return selectedDice;
    }

    private static ParseResult TryParse(string input)
    {
        var success = int.TryParse(input, out var result);
        return new ParseResult(success, result);
    }

    private static bool AllSuccessAndInRange(IEnumerable<DieRoll> validRolls, IList<ParseResult> parseResults)
    {
        var maxValue = validRolls.Count();
        return parseResults.All(it => it.Success) && parseResults.All(it => it.Value >= 0 && it.Value < maxValue);
    }

    public async Task<Rule> PickRuleToApply(Scoreboard board, IList<DieRoll> rolls)
    {
        Console.WriteLine("Current scores: ");
        Console.WriteLine(board.ToString());

        var success = false;
        var selectedRuleIndex = -1;
        while (!success)
        {
            Console.WriteLine("Pick a 0-indexed rule to apply");
            var picked = await AsyncConsole.ReadLineAsync();
            var result = TryParse(picked);
            success = result is { Success: true, Value: >= 0 } && result.Value < board.RulesWithScores.Count;
            if (success) selectedRuleIndex = result.Value;
        }
        return board[selectedRuleIndex].Rule;
    }

    private record struct ParseResult(bool Success, int Value);
}