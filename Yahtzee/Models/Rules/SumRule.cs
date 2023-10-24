namespace Yahtzee.Models.Rules;

internal class SumRule : Rule, IHasDependantRules
{

    public int[] DependantRulesIndices { get; }

    public SumRule(string nameIndex, string descriptionIndex, params int[] dependantRulesIndices) : base(nameIndex, descriptionIndex)
    {
        DependantRulesIndices = dependantRulesIndices;
    }

    public override Score Score(IList<DieRoll> rolls, Scoreboard board)
    {   
        return DependantRulesIndices.Select(i => board[i].Score).Sum();
    }

    public override bool Equals(object? obj)
    {
        return obj is SumRule rule &&
               base.Equals(obj) &&
               EqualityComparer<int[]>.Default.Equals(DependantRulesIndices, rule.DependantRulesIndices);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), DependantRulesIndices);
    }
}