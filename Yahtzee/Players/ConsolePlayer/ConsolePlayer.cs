using System.ComponentModel;
using Yahtzee.EventHandler;
using Yahtzee.Models;
using Yahtzee.Models.Rules;

namespace Yahtzee.Players.ConsolePlayer;

internal class ConsolePlayer : IPlayablePlayer
{
    public ConsolePlayer(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;


    public Task StartGame(GameState state)
    {
        Console.WriteLine("Starting game");
        state.Scoreboard.ScoreChanged += ScoreChanged;
        return Task.CompletedTask;
    }

    public Task EndGame(GameState state)
    {
        Console.WriteLine("End of game");
        state.Scoreboard.ScoreChanged -= ScoreChanged;
        return Task.CompletedTask;
    }

    private static void ScoreChanged(object sender, ScoreChangedEventArgs args)
    {
        Console.WriteLine($"Score changed: {args.ChangedScore.Rule.Name}: {args.ChangedScore.Score}");
        Console.WriteLine(args.Board.ToString());
    }

    public async Task<IList<DieRoll>> PickDiceToKeep(TurnState.RollTurnState state)
    {
        Console.WriteLine(ConsolePlayerStrings.ConsolePlayer_PickDiceToKeep_Current_roll___0_, state.ThrowCount);
        PrintRolls(state.KeptDice, state.LastRoll);
        
        var dice = state.KeptDice.Concat(state.LastRoll).ToList();
        var success = false;
        IList<DieRoll> selectedDice = new List<DieRoll>();
        while (!success)
        {
            Console.WriteLine(ConsolePlayerStrings.ConsolePlayer_PickDiceIndex);
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


    public async Task<IRule> PickRuleToApply(TurnState.PickRuleTurnState state)
    {
        Console.WriteLine(ConsolePlayerStrings.ConsolePlayer_PickRuleToApply_Current_scores);
        Console.WriteLine(state.Scoreboard.ToString());

        PrintRolls(state.KeptDice, null);
        
        var success = false;
        var selectedRuleIndex = -1;
        while (!success)
        {
            Console.WriteLine(ConsolePlayerStrings.ConsolePlayer_PickRuleToApply_Pick_a_0_indexed_rule_to_apply);
            var picked = await AsyncConsole.ReadLineAsync();
            var result = TryParse(picked);
            success = result is { Success: true, Value: >= 0 } && result.Value < state.Scoreboard.RulesWithScores.Count;
            if (success) selectedRuleIndex = result.Value;
        }
        return state.Scoreboard[selectedRuleIndex].Rule;
    }

    private static void PrintRolls(IList<DieRoll> keptRolls, IList<DieRoll>? newRolls)
    {
        var keptCount = keptRolls.Count;
        var rollLine = ConsolePlayerStrings.ConsolePlayer_PrintRolls_RollLine;
        if (keptCount > 0)
        {
            Console.WriteLine(ConsolePlayerStrings.ConsolePlayer_PrintRolls_Kept_dice_);
            foreach (var line in keptRolls.Select((roll, index) => string.Format(rollLine, index, roll.Value)))
            {
                Console.WriteLine(line);
            }
        }
        Console.WriteLine(ConsolePlayerStrings.ConsolePlayer_PrintRolls_New_rolls_);
        if (newRolls?.Any() == true)
        {
            foreach (var line in newRolls.Select((roll, index) => string.Format(rollLine, index + keptCount, roll.Value)))
            {
                Console.WriteLine(line);
            }
        }
    }

    private record struct ParseResult(bool Success, int Value);
}