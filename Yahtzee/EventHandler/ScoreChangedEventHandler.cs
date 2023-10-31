using Yahtzee.Models;

namespace Yahtzee.EventHandler;

public interface INotifyScoreChanged
{
    event ScoreChangedEventHandler? ScoreChanged;
}

public delegate void ScoreChangedEventHandler(object sender, ScoreChangedEventArgs e);

public class ScoreChangedEventArgs : EventArgs
{
    public ScoreChangedEventArgs(Scoreboard board, Scoreboard.RuleWithScore changedScore)
    {
        Board = board;
        ChangedScore = changedScore;
    }

    public Scoreboard Board { get; }
    public Scoreboard.RuleWithScore ChangedScore { get; }
}