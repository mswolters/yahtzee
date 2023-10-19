namespace Yahtzee.Models.Rules;

internal class FullHouseRule : Rule
{
    public FullHouseRule() : base("RuleNameFullHouse", "RuleDescriptionFullHouse")
    {
    }

    public override Score Score(IList<DieRoll> rolls, Scoreboard board)
    {
        var groupedRolls = rolls.GroupBy(roll => roll.Value).ToList();
        if (groupedRolls.Any(count => count.Count() == 3) && groupedRolls.Any(count => count.Count() == 2))
        {
            return new Score(25);
        }
        return new Score(0);
    }
    public override bool Equals(object? obj)
    {
        return obj is FullHouseRule &&
               base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode());
    }

}