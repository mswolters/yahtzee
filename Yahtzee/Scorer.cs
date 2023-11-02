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
        List<RuleId> topRules = new();
        for (var top = 1; top <= 6; top++)
        {
            var roll = new DieRoll(top);
            AddToListAndBoard(TopRuleId(new DieRoll(top)), new TopRule(roll), topRules, board);
        }

        Rule topSum = new SumRule("RuleNameTopSubsum", "RuleDescriptionTopSubsum", topRules.ToArray());
        board.AddRule(TopSubSumRuleId, topSum);
        Rule topBonus = new TopConditionRule(TopSubSumRuleId, 65, 35);
        board.AddRule(TopBonusRuleId, topBonus);
        Rule topTotalSum = new SumRule("RuleNameTopSum", "RuleDescriptionTopSum", TopSubSumRuleId, TopBonusRuleId);
        board.AddRule(TopSumRuleId, topTotalSum);

        List<RuleId> bottomRules = new();
        AddToListAndBoard(SameRuleId(3), new SameRule(3), bottomRules, board);
        AddToListAndBoard(SameRuleId(4), new SameRule(4), bottomRules, board);
        AddToListAndBoard(FullHouseRuleId, new FullHouseRule(), bottomRules, board);
        AddToListAndBoard(StraightRuleId(4), new StraightRule(4, 30), bottomRules, board);
        AddToListAndBoard(StraightRuleId(5), new StraightRule(5, 40), bottomRules, board);
        AddToListAndBoard(YahtzeeRuleId, new YahtzeeRule(), bottomRules, board);
        AddToListAndBoard(ChanceRuleId, new ChanceRule(), bottomRules, board);
        Rule bottomSum = new SumRule("RuleNameBottomSum", "RuleDescriptionBottomSum", bottomRules.ToArray());
        board.AddRule(BottomSumRuleId, bottomSum);

        Rule totalSum = new SumRule("RuleNameSum", "RuleDescriptionSum",  TopSumRuleId, BottomSumRuleId);
        board.AddRule(SumRuleId, totalSum);

        return board;
    }

    private static void AddToListAndBoard(RuleId id, Rule rule, List<RuleId> list, Scoreboard board)
    {
        list.Add(id);
        board.AddRule(id, rule);
    }

    public static RuleId TopRuleId(DieRoll roll) => new($"TOP{roll.Value}");
    public static RuleId TopSubSumRuleId = new("TOPSUBSUM");
    public static RuleId TopBonusRuleId = new("TOPBONUS");
    public static RuleId TopSumRuleId = new("TOPSUM");
    public static RuleId SameRuleId(int number) => new($"SAME{number}");
    public static RuleId FullHouseRuleId = new("FULLHOUSE");
    public static RuleId StraightRuleId(int length) => new($"STRAIGHT{length}");
    public static RuleId YahtzeeRuleId = new("YAHTZEE");
    public static RuleId ChanceRuleId = new("CHANCE");
    public static RuleId BottomSumRuleId = new("BOTTOMSUM");
    public static RuleId SumRuleId = new("SUM");
}