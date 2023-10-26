namespace Yahtzee.Models.Rules;

internal class TopConditionRule : Rule, IDependOnRules
{
    public int RuleIndex { get; }

    public int[] DependsOnIndices => new[] { RuleIndex };
    public int MinimumScore { get; }
    public int BonusScore { get; }

    public TopConditionRule(int ruleIndex, int minimumScore = 65, int bonusScore = 35) : base("RuleNameTopBonus", "RuleDescriptionTopBonus")
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
}