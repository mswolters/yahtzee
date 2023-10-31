using System.Text;
using Yahtzee.EventHandler;
using Yahtzee.Models.Rules;
using static Yahtzee.Models.Scoreboard;

namespace Yahtzee.Models;

/// <summary>
/// Models the scoreboard for a Yahtzee game.
/// </summary>
public class Scoreboard : INotifyScoreChanged
{
    // We can't use a dictionary as order is important
    private readonly List<RuleWithScore> _rulesWithScores;

    public Scoreboard()
    {
        _rulesWithScores = new List<RuleWithScore>();
    }

    public Scoreboard(IEnumerable<IRule> rules)
    {
        _rulesWithScores = rules.Select(RuleWithScore.DefaultForRule).ToList();
    }

    public Scoreboard(Scoreboard copy)
    {
        _rulesWithScores = copy.RulesWithScores.Select(rs => new RuleWithScore(rs.Rule, rs.Score)).ToList();
    }

    public record struct RuleWithScore(IRule Rule, Score Score)
    {
        internal static RuleWithScore DefaultForRule(IRule rule)
        {
            return new RuleWithScore(rule, new Score(0, false));
        }
    }

    public Score ScoreForRule(IRule rule)
    {
        return _rulesWithScores.Where(rs => Equals(rs.Rule, rule)).Select(rs => rs.Score).First();
    }

    public IList<IRule> Rules => _rulesWithScores.Select(rs => rs.Rule).ToList();

    public IList<RuleWithScore> RulesWithScores => new List<RuleWithScore>(_rulesWithScores);

    public void AddRule(IRule rule)
    {
        _rulesWithScores.Add(RuleWithScore.DefaultForRule(rule));
    }

    public void RemoveRule(IRule rule)
    {
        _rulesWithScores.RemoveAll(rs => Equals(rs.Rule, rule));
    }

    public void SetScore(IRule rule, Score score)
    {
        var index = _rulesWithScores.FindIndex(rs => Equals(rs.Rule, rule));
        var newRuleWithScore = _rulesWithScores[index] = new RuleWithScore(rule, score);
        ScoreChanged?.Invoke(this, new ScoreChangedEventArgs(this, newRuleWithScore));
        UpdateDependantScores(index);
    }

    public RuleWithScore this[int index] => _rulesWithScores[index];
    public RuleWithScore this[RuleId index] => _rulesWithScores.Find(rs => rs.Rule.Id == index);

    public Score this[IRule key]
    {
        get => ScoreForRule(key);
        set => SetScore(key, value);
    }

    private void UpdateDependantScores(int index)
    {
        var rulesWhichDependsOnIndex = _rulesWithScores
            .Select(rs => rs.Rule)
            .OfType<IDependOnRules>()
            .Where(hasDependants => hasDependants.DependsOnIndices.Contains(index))
            .ToList(); // Prevent modification of the underlying collection while enumerating by forcing the enumeration to run to the end
        foreach (var ruleWhichDependsOnIndex in rulesWhichDependsOnIndex)
        {
            SetScore(ruleWhichDependsOnIndex, ruleWhichDependsOnIndex.Score(new List<DieRoll>(), this));
        }
    }

    private readonly ScoreboardWriter _writer = new(16, 5);

    public override string ToString()
    {
        return _writer.Stringify(this);
    }

    public event ScoreChangedEventHandler? ScoreChanged;
}

internal record ScoreboardWriter(int RuleWidth, int ScoreWidth)
{
    public string Stringify(Scoreboard scoreboard)
    {
        var sb = new StringBuilder();
        sb.Append('|');
        sb.Append(PadCenter("Name", RuleWidth));
        sb.Append('|');
        sb.Append(PadCenter("Score", ScoreWidth));
        sb.Append('|');
        sb.AppendLine();

        foreach (var rs in scoreboard.RulesWithScores)
        {
            AppendRuleWithScore(sb, rs);
        }

        return sb.ToString();
    }

    private void AppendRuleWithScore(StringBuilder sb, RuleWithScore rs)
    {
        sb.Append('|');
        sb.Append(PadCenter(rs.Rule.Name, RuleWidth));
        sb.Append('|');
        sb.Append(ScoreString(rs.Score));
        sb.Append('|');
        sb.AppendLine();
    }

    private string ScoreString(Score score)
    {
        if (score is { Value: 0, Written: false })
        {
            return PadCenter("-", ScoreWidth);
        }

        if (score.Written)
        {
            return score.Value.ToString().PadLeft(ScoreWidth - 1).PadRight(ScoreWidth);
        }

        return ("{" + score.Value + "}").PadLeft(ScoreWidth);
    }

    private static string PadCenter(string str, int length, char padChar = ' ')
    {
        var strLength = str.Length;
        if (strLength > length)
        {
            str = str[..length];
            strLength = length;
        }

        var numPadChars = length - strLength;
        var padLeft = length - numPadChars / 2;
        return str.PadLeft(padLeft, padChar).PadRight(length);
    }
}

/// <summary>
/// The score that is used on the scoreboard.
/// The written value signifies the finality of a score. Written values should be treated as virtually unchangeable.
/// </summary>
public readonly record struct Score(int Value, bool Written = true)
{
    public static Score operator +(Score left, Score right)
    {
        return new Score(left.Value + right.Value, left.Written && right.Written);
    }
}

internal static class ScoreExtensions
{
    public static Score Sum(this IEnumerable<Score> values)
    {
        return values.Aggregate(new Score(0, true), (total, next) => total + next);
    }
}