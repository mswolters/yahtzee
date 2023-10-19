using System.ComponentModel;
using Yahtzee.Models.Rules;

namespace Yahtzee.Models;

public interface IPlayer : INotifyPropertyChanged
{
    public string Name { get; set; }

}

internal interface IPlayablePlayer : IPlayer
{

    public Task<IList<DieRoll>> PickDiceToKeep(TurnState state, IList<DieRoll> rolls);
    public Task<Rule> PickRuleToApply(Scoreboard board, IList<DieRoll> rolls);

}