using System.Text;
using Yahtzee.EventHandler;
using Yahtzee.Players;

namespace Yahtzee.Models;

public class MultiScoreboard : INotifyPlayerScoreChanged
{
    public List<IPlayer> Players { get; }
    public List<SingleScoreboard.IdRule> Rules { get; }

    private readonly Dictionary<IPlayer, SingleScoreboard> _singleScoreboards = new();

    public MultiScoreboard(List<IPlayer> players, List<SingleScoreboard.IdRule> rules)
    {
        Rules = rules;
        Players = players;
        foreach (var player in players)
        {
            var childBoard = new SingleScoreboard(rules);
            childBoard.ScoreChanged += (_, args) => ChildScoreChanged(player, args);
            _singleScoreboards[player] = childBoard;
        }
    }

    public MultiScoreboard(List<IPlayer> players, SingleScoreboard board)
        : this(players, board.RulesWithScores.Select(rs => new SingleScoreboard.IdRule(rs.Id, rs.Rule)).ToList())
    {
    }

    public SingleScoreboard this[IPlayer player] => _singleScoreboards[player];

    private void ChildScoreChanged(IPlayer player, ScoreChangedEventArgs args)
    {
        PlayerScoreChanged?.Invoke(this, new PlayerScoreChangedEventArgs(player, this, args.ChangedScore));
    }

    public event PlayerScoreChangedEventHandler? PlayerScoreChanged;

    private readonly MultiScoreboardWriter _writer = new(16, 5);

    public override string ToString()
    {
        return _writer.Stringify(this);
    }
}

internal record MultiScoreboardWriter(int RuleWidth, int ScoreWidth)
{
    public string Stringify(MultiScoreboard scoreboard)
    {
        var sb = new StringBuilder();
        sb.Append('|');
        sb.Append(' ', RuleWidth);
        sb.Append('|');
        foreach (var player in scoreboard.Players)
        {
            sb.Append(player.Name);
            sb.Append('|');
        }

        foreach (var rule in scoreboard.Rules)
        {
            AppendRuleAndScoresPerPlayer(sb, scoreboard, rule);
        }

        return sb.ToString();
    }

    private void AppendRuleAndScoresPerPlayer(StringBuilder sb, MultiScoreboard scoreboard,
        SingleScoreboard.IdRule rule)
    {
        sb.Append('|');
        sb.Append(rule.Rule.Name.PadCenter(RuleWidth));
        sb.Append('|');
        foreach (var player in scoreboard.Players)
        {
            sb.Append(ScoreString(scoreboard[player][rule.Id].Score));
            sb.Append('|');
        }

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