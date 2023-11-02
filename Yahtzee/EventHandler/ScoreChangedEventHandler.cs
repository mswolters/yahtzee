using Yahtzee.Models;
using Yahtzee.Players;

namespace Yahtzee.EventHandler;

public interface INotifyScoreChanged
{
    event ScoreChangedEventHandler? ScoreChanged;
}

public delegate void ScoreChangedEventHandler(object sender, ScoreChangedEventArgs e);

public class ScoreChangedEventArgs : EventArgs
{
    public ScoreChangedEventArgs(SingleScoreboard board, SingleScoreboard.RuleWithScore changedScore)
    {
        Board = board;
        ChangedScore = changedScore;
    }

    public SingleScoreboard Board { get; }
    public SingleScoreboard.RuleWithScore ChangedScore { get; }
}

public interface INotifyPlayerScoreChanged
{
    event PlayerScoreChangedEventHandler? PlayerScoreChanged;
}

public delegate void PlayerScoreChangedEventHandler(object sender, PlayerScoreChangedEventArgs e);

public class PlayerScoreChangedEventArgs : EventArgs
{
    public IPlayer Player { get; }
    public MultiScoreboard FullBoard { get; }
    public SingleScoreboard.RuleWithScore ChangedScore { get; }

    public PlayerScoreChangedEventArgs(
        IPlayer player,
        MultiScoreboard fullBoard,
        SingleScoreboard.RuleWithScore changedScore
    )
    {
        Player = player;
        FullBoard = fullBoard;
        ChangedScore = changedScore;
    }
}