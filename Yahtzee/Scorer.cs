using Yahtzee.Models;
using Yahtzee.Models.Rules;

namespace Yahtzee;

internal static class Scorer
{
    private static Scoreboard? _defaultBoard = null;
    internal static Scoreboard DefaultBoard
    {
        get
        {
            _defaultBoard ??= BuildDefaultBoard();
            return new(_defaultBoard);
        }
    }

    private static Scoreboard BuildDefaultBoard()
    {
        Scoreboard board = new();
        List<Rule> topRules = new();
        for (var top = 1; top <= 6; top++)
        {
            var roll = new DieRoll(top);
            AddToListAndBoard(new TopRule(TopRuleId(roll), roll), topRules, board);
        }
        Rule topSum = new SumRule(TopSubSumRuleId,"RuleNameTopSubsum", "RuleDescriptionTopSubsum", board.Rules.ToList().IndicesOf(topRules).ToArray());
        board.AddRule(topSum);
        Rule topBonus = new TopConditionRule(TopBonusRuleId, board.Rules.ToList().IndexOf(topSum), 65, 35);
        board.AddRule(topBonus);
        Rule topTotalSum = new SumRule(TopSumRuleId,"RuleNameTopSum", "RuleDescriptionTopSum", board.Rules.ToList().IndicesOf(topSum, topBonus).ToArray());
        board.AddRule(topTotalSum);

        List<Rule> bottomRules = new();
        AddToListAndBoard(new SameRule(SameRuleId(3), 3), bottomRules, board);
        AddToListAndBoard(new SameRule(SameRuleId(4), 4), bottomRules, board);
        AddToListAndBoard(new FullHouseRule(FullHouseRuleId), bottomRules, board);
        AddToListAndBoard(new StraightRule(StraightRuleId(4), 4, 30), bottomRules, board);
        AddToListAndBoard(new StraightRule(StraightRuleId(5), 5, 40), bottomRules, board);
        AddToListAndBoard(new YahtzeeRule(YahtzeeRuleId), bottomRules, board);
        AddToListAndBoard(new ChanceRule(ChanceRuleId), bottomRules, board);
        Rule bottomSum = new SumRule(BottomSumRuleId,"RuleNameBottomSum", "RuleDescriptionBottomSum", board.Rules.ToList().IndicesOf(bottomRules).ToArray());
        board.AddRule(bottomSum);

        Rule totalSum = new SumRule(SumRuleId,"RuleNameSum", "RuleDescriptionSum", board.Rules.ToList().IndicesOf(topTotalSum, bottomSum).ToArray());
        board.AddRule(totalSum);

        return board;
    }

    private static void AddToListAndBoard(Rule rule, List<Rule> list, Scoreboard board)
    {
        list.Add(rule);
        board.AddRule(rule);
    }

    public static RuleId TopRuleId(DieRoll roll) => new($"TOP{roll.Value}");
    public static RuleId TopSubSumRuleId = new("TOPSUBUSUM");
    public static RuleId TopBonusRuleId = new("TOPBONUS");
    public static RuleId TopSumRuleId = new("TOPUSUM");
    public static RuleId SameRuleId(int number) => new($"SAME{number}");
    public static RuleId FullHouseRuleId = new("FULLHOUSE");
    public static RuleId StraightRuleId(int length) => new($"STRAIGHT{length}");
    public static RuleId YahtzeeRuleId = new("YAHTZEE");
    public static RuleId ChanceRuleId = new("CHANCE");
    public static RuleId BottomSumRuleId = new("BOTTOMSUM");
    public static RuleId SumRuleId = new("SUM");
}