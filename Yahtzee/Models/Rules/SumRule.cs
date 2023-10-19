namespace Yahtzee.Models.Rules;

internal class SumRule : Rule
{
    public int[] RulesIndices { get; }

    public SumRule(string nameIndex, string descriptionIndex, params int[] rulesIndices) : base(nameIndex, descriptionIndex)
    {
        RulesIndices = rulesIndices;
    }

    public override Score Score(IList<DieRoll> rolls, Scoreboard board)
    {   
        return RulesIndices.Select(i => board[i].Score).Sum();
    }

    public override bool Equals(object? obj)
    {
        return obj is SumRule rule &&
               base.Equals(obj) &&
               EqualityComparer<int[]>.Default.Equals(RulesIndices, rule.RulesIndices);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), RulesIndices);
    }
}