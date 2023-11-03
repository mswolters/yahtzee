using System.Text;
using Yahtzee.EventHandler;
using Yahtzee.Models.Rules;
using static Yahtzee.Models.SingleScoreboard;

namespace Yahtzee.Models;

/// <summary>
/// Models the scoreboard for a Yahtzee game.
/// </summary>
public class SingleScoreboard : INotifyScoreChanged
{
    // We can't use a dictionary as order is important
    private readonly List<RuleWithScore> _rulesWithScores;

    public SingleScoreboard()
    {
        _rulesWithScores = new List<RuleWithScore>();
    }

    public SingleScoreboard(IEnumerable<IdRule> rules)
    {
        _rulesWithScores =
            new List<RuleWithScore>(rules.Select(idRule =>
                new RuleWithScore(idRule.Id, idRule.Rule, new Score(0, false))
            ));
    }

    public SingleScoreboard(IEnumerable<RuleWithScore> rules)
    {
        _rulesWithScores = new List<RuleWithScore>(rules);
    }

    public SingleScoreboard(SingleScoreboard copy)
    {
        _rulesWithScores = new List<RuleWithScore>(copy.RulesWithScores);
    }

    public readonly record struct IdRule(RuleId Id, IRule Rule);

    public readonly record struct RuleWithScore(RuleId Id, IRule Rule, Score Score);

    public IList<IRule> Rules => _rulesWithScores.Select(rs => rs.Rule).ToList();

    public IList<RuleWithScore> RulesWithScores => new List<RuleWithScore>(_rulesWithScores);

    public void SetScore(RuleId id, Score score)
    {
        var index = _rulesWithScores.FindIndex(rs => Equals(rs.Id, id));
        var newRuleWithScore = _rulesWithScores[index] = new RuleWithScore(id, _rulesWithScores[index].Rule, score);
        ScoreChanged?.Invoke(this, new ScoreChangedEventArgs(this, newRuleWithScore));
        UpdateDependantScores(id);
    }

    public RuleWithScore this[int index] => _rulesWithScores[index];
    public RuleWithScore this[RuleId index] => _rulesWithScores.Find(rs => rs.Id == index);

    private void UpdateDependantScores(RuleId id)
    {
        var rulesWhichDependOn = _rulesWithScores.Where(rs =>
            {
                if (rs.Rule is IDependOnRules d)
                {
                    return d.DependsOnIds.Contains(id);
                }

                return false;
            })
            .Select(rs => rs.Id)
            .ToList();
        foreach (var ruleIdOfDependant in rulesWhichDependOn)
        {
            Poke(ruleIdOfDependant);
        }
    }

    // Forces a score to reset to calculated value
    private void Poke(RuleId id)
    {
        SetScore(id, this[id].Rule.Score(new List<DieRoll>(), this));
    }

    private readonly SingleScoreboardWriter _writer = new(16, 5);

    public override string ToString()
    {
        return _writer.Stringify(this);
    }

    public event ScoreChangedEventHandler? ScoreChanged;
}

internal record SingleScoreboardWriter(int RuleWidth, int ScoreWidth)
{
    public string Stringify(SingleScoreboard scoreboard)
    {
        var sb = new StringBuilder();
        sb.Append('|');
        sb.Append("Name".PadCenter(RuleWidth));
        sb.Append('|');
        sb.Append("Score".PadCenter(ScoreWidth));
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
        sb.Append(rs.Rule.Name.PadCenter(RuleWidth));
        sb.Append('|');
        sb.Append(ScoreString(rs.Score));
        sb.Append('|');
        sb.AppendLine();
    }

    private string ScoreString(Score score)
    {
        if (score is { Value: 0, Written: false })
        {
            return "-".PadCenter(ScoreWidth);
        }

        if (score.Written)
        {
            return score.Value.ToString().PadLeft(ScoreWidth - 1).PadRight(ScoreWidth);
        }

        return ("{" + score.Value + "}").PadLeft(ScoreWidth);
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