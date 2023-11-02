namespace Yahtzee.Models.Rules;

internal class SameRule : Rule
{
    public int Number { get; }


    public SameRule(int number) : base($"RuleNameSame{number}", $"RuleDescriptionSame{number}")
    {
        Number = number;
    }

    public override Score Score(IList<DieRoll> rolls, SingleScoreboard board)
    {
        if (rolls.GroupBy(roll => roll.Value).Any(rollOfNumber => rollOfNumber.Count() >= Number))
        {
            return new Score(rolls.Sum(roll => roll.Value));
        }
        return new Score(0);
    }

    public override bool Equals(object? obj)
    {
        return obj is SameRule rule &&
               base.Equals(obj) &&
               Number == rule.Number;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Number);
    }
}