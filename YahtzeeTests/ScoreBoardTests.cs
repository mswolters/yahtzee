using Yahtzee.Models;
using Yahtzee.Models.Rules;

namespace Yahtzee.Tests;

public class ScoreBoardTests
{

    private static readonly RuleId TopRuleId = new RuleId("a");
    private static readonly RuleId SumRuleId = new RuleId("s");
    
    [TestMethod]
    public void SumRuleIsUpdatedWhenOtherRuleIsUpdated()
    {
        var scoreboard = new SingleScoreboard(new List<SingleScoreboard.RuleWithScore>
        {
            new(TopRuleId, new TopRule(new DieRoll(1)), new Score()), 
            new(SumRuleId, new SumRule("", "", SumRuleId), new Score()),
        });
        scoreboard.SetScore(scoreboard.Rules[0], new Score(3, true));
        Assert.AreEqual(new Score(3, true), scoreboard[1].Score);
    }
    
}