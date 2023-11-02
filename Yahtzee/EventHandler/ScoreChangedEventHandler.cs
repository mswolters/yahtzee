using Yahtzee.Models;

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