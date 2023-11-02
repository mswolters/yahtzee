using Yahtzee.Models;
using Yahtzee.Models.Rules;

namespace Yahtzee;

internal static class Scorer
{
    private static SingleScoreboard? _defaultBoard = null;
    internal static SingleScoreboard DefaultBoard
    {
        get
        {
            _defaultBoard ??= BuildDefaultBoard();
            return new(_defaultBoard);
        }
    }

    private static SingleScoreboard BuildDefaultBoard()
    {
        List<SingleScoreboard.IdRule> rules = new(); 
        
        List<RuleId> topRuleIds = new();
        for (var top = 1; top <= 6; top++)
        {
            var roll = new DieRoll(top);
            AddToList(TopRuleId(new DieRoll(top)), new TopRule(roll), rules, topRuleIds);
        }

        Rule topSum = new SumRule("RuleNameTopSubsum", "RuleDescriptionTopSubsum", topRuleIds.ToArray());
        AddToList(TopSubSumRuleId, topSum, rules);
        Rule topBonus = new TopConditionRule(TopSubSumRuleId, 65, 35);
        AddToList(TopBonusRuleId, topBonus, rules);
        Rule topTotalSum = new SumRule("RuleNameTopSum", "RuleDescriptionTopSum", TopSubSumRuleId, TopBonusRuleId);
        AddToList(TopSumRuleId, topTotalSum, rules);

        List<RuleId> bottomRules = new();
        AddToList(SameRuleId(3), new SameRule(3), rules, bottomRules);
        AddToList(SameRuleId(4), new SameRule(4), rules, bottomRules);
        AddToList(FullHouseRuleId, new FullHouseRule(), rules, bottomRules);
        AddToList(StraightRuleId(4), new StraightRule(4, 30), rules, bottomRules);
        AddToList(StraightRuleId(5), new StraightRule(5, 40), rules, bottomRules);
        AddToList(YahtzeeRuleId, new YahtzeeRule(), rules, bottomRules);
        AddToList(ChanceRuleId, new ChanceRule(), rules, bottomRules);
        Rule bottomSum = new SumRule("RuleNameBottomSum", "RuleDescriptionBottomSum", bottomRules.ToArray());
        AddToList(BottomSumRuleId, bottomSum, rules);

        Rule totalSum = new SumRule("RuleNameSum", "RuleDescriptionSum",  TopSumRuleId, BottomSumRuleId);
        AddToList(SumRuleId, totalSum, rules);

        SingleScoreboard board = new(rules);
        return board;
    }

    private static void AddToList(RuleId id, IRule rule, List<SingleScoreboard.IdRule> list, List<RuleId>? idList = null)
    {
        list.Add(new SingleScoreboard.IdRule(id, rule));
        idList?.Add(id);
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