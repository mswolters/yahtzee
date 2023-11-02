using Yahtzee.EventHandler;
using Yahtzee.Players;

namespace Yahtzee.Models;

public class MultiScoreboard : INotifyPlayerScoreChanged
{
    public List<IPlayer> Players { get; }

    private readonly Dictionary<IPlayer, SingleScoreboard> _singleScoreboards = new();

    public MultiScoreboard(List<IPlayer> players, List<SingleScoreboard.IdRule> rules)
        : this(players, new SingleScoreboard(rules))
    {
    }

    public MultiScoreboard(List<IPlayer> players, SingleScoreboard board)
    {
        Players = players;
        foreach (var player in players)
        {
            var childBoard = new SingleScoreboard(board);
            childBoard.ScoreChanged += (_, args) => ChildScoreChanged(player, args);
            _singleScoreboards[player] = childBoard;
        }
    }

    public SingleScoreboard this[IPlayer player] => _singleScoreboards[player];

    private void ChildScoreChanged(IPlayer player, ScoreChangedEventArgs args)
    {
        PlayerScoreChanged?.Invoke(this, new PlayerScoreChangedEventArgs(player, this, args.ChangedScore));
    }

    public event PlayerScoreChangedEventHandler? PlayerScoreChanged;
}