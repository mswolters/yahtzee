using Yahtzee.Models;
using Yahtzee.Models.Rules;

namespace Yahtzee.Tests;

public class ScoreBoardTests
{

    [TestMethod]
    public void SumRuleIsUpdatedWhenOtherRuleIsUpdated()
    {
        var scoreboard = new Scoreboard(new List<IRule> { new TopRule(new RuleId(""), new DieRoll(1)), new SumRule(new RuleId(""), "", "", 0) });
        scoreboard.SetScore(scoreboard.Rules[0], new Score(3, true));
        Assert.AreEqual(new Score(3, true), scoreboard[1].Score);
    }
    
}