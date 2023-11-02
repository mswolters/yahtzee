namespace Yahtzee.Models.Rules;

internal class TopRule : Rule
{

    public DieRoll Roll { get; }

    public TopRule(DieRoll roll) : base($"RuleName{roll}s", $"RuleDescription{roll}s")
    {
        Roll = roll;
    }

    public override Score Score(IList<DieRoll> rolls, SingleScoreboard board) => new(rolls.Where(x => x == Roll).Sum(x => x.Value));

    public override bool Equals(object? obj)
    {
        return obj is TopRule rule &&
               base.Equals(obj) &&
               Roll.Equals(rule.Roll);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Roll);
    }
}