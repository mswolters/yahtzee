namespace Yahtzee.Models.Rules;

internal class ChanceRule : Rule
{
    public ChanceRule() : base("RuleNameChance", "RuleDescriptionChance")
    {
    }

    public override Score Score(IList<DieRoll> rolls, Scoreboard board) => new(rolls.Sum(roll => roll.Value));
    public override bool Equals(object? obj)
    {
        return obj is ChanceRule &&
               base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode());
    }
}