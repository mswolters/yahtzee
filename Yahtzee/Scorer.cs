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
            return new Scoreboard(_defaultBoard);
        }
    }

    private static Scoreboard BuildDefaultBoard()
    {
        Scoreboard board = new();
        List<Rule> topRules = new();
        for (var top = 1; top <= 6; top++)
        {
            AddToListAndBoard(new TopRule(new DieRoll(top)), topRules, board);
        }
        Rule topSum = new SumRule("RuleNameTopSubsum", "RuleDescriptionTopSubsum", board.Rules.ToList().IndicesOf(topRules).ToArray());
        board.AddRule(topSum);
        Rule topBonus = new TopConditionRule(board.Rules.ToList().IndexOf(topSum), 65, 35);
        board.AddRule(topBonus);
        Rule topTotalSum = new SumRule("RuleNameTopSum", "RuleDescriptionTopSum", board.Rules.ToList().IndicesOf(topSum, topBonus).ToArray());
        board.AddRule(topTotalSum);

        List<Rule> bottomRules = new();
        AddToListAndBoard(new SameRule(3), bottomRules, board);
        AddToListAndBoard(new FullHouseRule(), bottomRules, board);
        AddToListAndBoard(new StraightRule(4, 30), bottomRules, board);
        AddToListAndBoard(new StraightRule(5, 40), bottomRules, board);
        AddToListAndBoard(new YahtzeeRule(), bottomRules, board);
        AddToListAndBoard(new ChanceRule(), bottomRules, board);
        Rule bottomSum = new SumRule("RuleNameBottomSum", "RuleDescriptionBottomSum", board.Rules.ToList().IndicesOf(bottomRules).ToArray());
        board.AddRule(bottomSum);

        Rule totalSum = new SumRule("RuleNameSum", "RuleDescriptionSum", board.Rules.ToList().IndicesOf(topTotalSum, bottomSum).ToArray());
        board.AddRule(totalSum);

        return board;
    }

    private static void AddToListAndBoard(Rule rule, List<Rule> list, Scoreboard board)
    {
        list.Add(rule);
        board.AddRule(rule);
    }
}