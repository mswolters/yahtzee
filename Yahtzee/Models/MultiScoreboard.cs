using Yahtzee.Players;

namespace Yahtzee.Models;

public class MultiScoreboard
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
            _singleScoreboards[player] = childBoard;
        }
    }

    public SingleScoreboard this[IPlayer player] => _singleScoreboards[player];
}