namespace Yahtzee.Models.Rules;

internal class SumRule : Rule, IDependOnRules
{

    public int[] DependsOnIndices { get; }

    public SumRule(RuleId id, string nameIndex, string descriptionIndex, params int[] dependantRulesIndices) : base(id, nameIndex, descriptionIndex)
    {
        DependsOnIndices = dependantRulesIndices;
    }

    public override Score Score(IList<DieRoll> rolls, Scoreboard board)
    {   
        return DependsOnIndices.Select(i => board[i].Score).Sum();
    }

    public override bool Equals(object? obj)
    {
        return obj is SumRule rule &&
               base.Equals(obj) &&
               EqualityComparer<int[]>.Default.Equals(DependsOnIndices, rule.DependsOnIndices);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), DependsOnIndices);
    }
}