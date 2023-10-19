using Yahtzee.Models;

internal static class DiceTestsHelpers
{

    public static IList<DieRoll> RollsOf(params int[] rolls)
    {
        return rolls.Select(roll => new DieRoll(roll)).ToList();
    }
}