using System.ComponentModel;
using Yahtzee.Models.Rules;

namespace Yahtzee.Models;

public interface IPlayer : INotifyPropertyChanged
{
    public string Name { get; set; }

}

internal interface IPlayablePlayer : IPlayer
{

    public Task<IList<DieRoll>> PickDiceToKeep(TurnState.RollTurnState state);
    public Task<Rule> PickRuleToApply(TurnState.PickRuleTurnState state);

}