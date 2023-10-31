using System.ComponentModel;
using Yahtzee.Models;
using Yahtzee.Models.Rules;

namespace Yahtzee.Players;

public interface IPlayer : INotifyPropertyChanged
{
    public string Name { get; set; }

}

internal interface IPlayablePlayer : IPlayer
{

    public Task StartGame(GameState state);
    public Task EndGame(GameState state);
    
    public Task<IList<DieRoll>> PickDiceToKeep(TurnState.RollTurnState state);
    public Task<IRule> PickRuleToApply(TurnState.PickRuleTurnState state);

}