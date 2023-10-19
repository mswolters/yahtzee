using Yahtzee.Models;

namespace Yahtzee.EventHandler;

public interface INotifyDiceRolled
{
    event DiceRolledEventHandler? DiceRolled;
}

public delegate void DiceRolledEventHandler(object sender, DiceRolledEventArgs e);

public class DiceRolledEventArgs : EventArgs { 

    public DiceRolledEventArgs(IPlayer player, IList<DieRoll> rolls)
    {
        Player = player;
        Rolls = rolls;
    }

    public IPlayer Player { get; }
    public IList<DieRoll> Rolls { get; }
}

public interface INotifyDiceKept
{
    event DiceKeptEventHandler? DiceKept;
}

public delegate void DiceKeptEventHandler(object sender, DiceRolledEventArgs e);

public class DiceKeptEventArgs : EventArgs
{
    public DiceKeptEventArgs(IPlayer player, IList<DieRoll> rolls)
    {
        Player = player;
        Rolls = rolls;
    }

    public IPlayer Player { get; }
    public IList<DieRoll> Rolls { get; }
}