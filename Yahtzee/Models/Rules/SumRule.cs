namespace Yahtzee.Models.Rules;

internal class SumRule : Rule, IDependOnRules
{

    public RuleId[] DependsOnIds { get; }

    public SumRule(string nameIndex, string descriptionIndex, params RuleId[] dependantRulesIndices) : base(nameIndex, descriptionIndex)
    {
        DependsOnIds = dependantRulesIndices;
    }

    public override Score Score(IList<DieRoll> rolls, Scoreboard board)
    {   
        return DependsOnIds.Select(i => board[i].Score).Sum();
    }

    public override bool Equals(object? obj)
    {
        return obj is SumRule rule &&
               base.Equals(obj) &&
               EqualityComparer<RuleId[]>.Default.Equals(DependsOnIds, rule.DependsOnIds);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), DependsOnIds);
    }
}