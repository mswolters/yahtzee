using Yahtzee;
using Yahtzee.Models;
using Yahtzee.Models.Rules;

namespace YahtzeeTests;

[TestClass]
public class RuleTests
{

    private Scoreboard scoreboard = new Scoreboard();

    [TestInitialize]
    public void Setup() {
        scoreboard = Scorer.DefaultBoard;
    }

    [TestMethod]
    public void TopRuleTest()
    {
        var threes = new TopRule(new DieRoll(3));
        Assert.AreEqual(new Score(6), threes.Score(DiceTestsHelpers.RollsOf(1, 2, 3, 3, 6), scoreboard));
    }

    [TestMethod]
    public void StraightRuleLengthMatchesTest()
    {
        var straight = new StraightRule(4, 30);
        Assert.AreEqual(new Score(30), straight.Score(DiceTestsHelpers.RollsOf(1, 2, 3, 4, 6), scoreboard));
        Assert.AreEqual(new Score(30), straight.Score(DiceTestsHelpers.RollsOf(1, 3, 4, 5, 6), scoreboard));
    }

    [TestMethod]
    public void StraightRuleLongerMatchesTest()
    {
        var straight = new StraightRule(4, 30);
        Assert.AreEqual(new Score(30), straight.Score(DiceTestsHelpers.RollsOf(1, 2, 3, 4, 5), scoreboard));
    }

    [TestMethod]
    public void StraightRuleTooShortDoesNotMatchTest()
    {
        var straight = new StraightRule(4, 30);
        Assert.AreEqual(new Score(0), straight.Score(DiceTestsHelpers.RollsOf(1, 2, 3, 5, 6), scoreboard));
    }

    [TestMethod]
    public void StraightRuleWithDuplicatesMatchesTest()
    {
        var straight = new StraightRule(4, 30);
        Assert.AreEqual(new Score(30), straight.Score(DiceTestsHelpers.RollsOf(3, 2, 5, 3, 4, 2), scoreboard));
    }

    [TestMethod]
    public void SameRuleMatchesTest()
    {
        var same4 = new SameRule(4);
        Assert.AreEqual(new Score(17), same4.Score(DiceTestsHelpers.RollsOf(4, 4, 4, 4, 1), scoreboard));
    }

    [TestMethod]
    public void SameRuleDoesNotMatchTest()
    {
        var same4 = new SameRule(4);
        Assert.AreEqual(new Score(0), same4.Score(DiceTestsHelpers.RollsOf(4, 4, 4, 5, 2), scoreboard));
    }

    [TestMethod]
    public void FullHouseRuleMatchesTest()
    {
        var fullHouse = new FullHouseRule();
        Assert.AreEqual(new Score(25), fullHouse.Score(DiceTestsHelpers.RollsOf(3, 2, 3, 2, 3), scoreboard));
    }

    [TestMethod]
    public void FullHouseRuleDoesNotMatchTest()
    {
        var fullHouse = new FullHouseRule();
        Assert.AreEqual(new Score(0), fullHouse.Score(DiceTestsHelpers.RollsOf(3, 2, 4, 2, 3), scoreboard));
    }

    [TestMethod]
    public void FullHouseRuleYahtzeeDoesNotMatchTest()
    {
        var fullHouse = new FullHouseRule();
        Assert.AreEqual(new Score(0), fullHouse.Score(DiceTestsHelpers.RollsOf(3, 3, 3, 3, 3), scoreboard));
    }

    [TestMethod]
    public void YahtzeeRuleMatchesTest()
    {
        var yahtzee = new YahtzeeRule();
        Assert.AreEqual(new Score(50), yahtzee.Score(DiceTestsHelpers.RollsOf(3, 3, 3, 3, 3), scoreboard));
    }

    [TestMethod]
    public void YahtzeeRuleDoesNotMatchTest()
    {
        var yahtzee = new YahtzeeRule();
        Assert.AreEqual(new Score(0), yahtzee.Score(DiceTestsHelpers.RollsOf(3, 3, 2, 3, 3), scoreboard));
    }
}