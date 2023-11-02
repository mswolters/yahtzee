namespace Yahtzee.Models.Rules;

internal class TopConditionRule : Rule, IDependOnRules
{
    public RuleId RuleIndex { get; }

    public RuleId[] DependsOnIds => new[] { RuleIndex };
    public int MinimumScore { get; }
    public int BonusScore { get; }

    public TopConditionRule(RuleId ruleIndex, int minimumScore = 65, int bonusScore = 35) : base("RuleNameTopBonus", "RuleDescriptionTopBonus")
    {
        RuleIndex = ruleIndex;
        MinimumScore = minimumScore;
        BonusScore = bonusScore;
    }

    public override Score Score(IList<DieRoll> rolls, Scoreboard board)
    {
        var otherScore = board[RuleIndex].Score;
        return otherScore with { Value = otherScore.Value >= MinimumScore ? BonusScore : 0 };
    }

    protected bool Equals(TopConditionRule other)
    {
        return base.Equals(other) 
               && RuleIndex.Equals(other.RuleIndex) 
               && MinimumScore == other.MinimumScore 
               && BonusScore == other.BonusScore;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TopConditionRule)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), RuleIndex, MinimumScore, BonusScore);
    }
}