using System.ComponentModel;
using Yahtzee.Models;
using Yahtzee.Models.Rules;

namespace Yahtzee.Players;

public interface IPlayer : INotifyPropertyChanged
{
    public Guid Id { get; init; }
    public string Name { get; set; }

}

public interface IPlayablePlayer : IPlayer
{

    public Task StartGame(MultiScoreboard board);
    public Task EndGame(MultiScoreboard board);
    
    public Task<IList<DieRoll>> PickDiceToKeep(TurnState.RollTurnState state);
    public Task<RuleId> PickRuleToApply(TurnState.PickRuleTurnState state);

}