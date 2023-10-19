namespace Yahtzee.Models.Rules;

internal class StraightRule : Rule
{
    public int Length { get; }
    public int Value { get; }

    public StraightRule(int length, int value) : base($"RuleNameStraight{length}", $"RuleDescriptionStraight{length}")
    {
        Length = length;
        Value = value;
    }

    public override Score Score(IList<DieRoll> rolls, Scoreboard board)
    {
        var orderedDistinctRolls = rolls.OrderBy(roll => roll.Value).Distinct();
        var maxOrderLength = 1;
        var currentOrderLength = 0;
        var previousNumber = int.MaxValue;
        foreach (var roll in orderedDistinctRolls)
        {
            if (roll.Value == previousNumber + 1)
            {
                currentOrderLength++;
                if (currentOrderLength > maxOrderLength)
                {
                    maxOrderLength = currentOrderLength;
                }
            }
            else
            {
                currentOrderLength = 1;
            }
            previousNumber = roll.Value;
        }
        return new Score((maxOrderLength >= Length) ? Value : 0);
    }

    public override bool Equals(object? obj)
    {
        return obj is StraightRule rule &&
               base.Equals(obj) &&
               Length == rule.Length &&
               Value == rule.Value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Length, Value);
    }
}