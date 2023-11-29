using System.ComponentModel;
using Yahtzee.Models;
using Yahtzee.Models.Rules;
using Yahtzee.Players;

namespace Yahtzee.Tests.Mock;

internal class MockPlayer : IPlayablePlayer
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get => "MockPlayer"; set => throw new NotImplementedException(); }
    public bool StartGameWasCalled { get; set; }
    public bool EndGameWasCalled { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private int[] _keptIndices = Array.Empty<int>();
    public void KeepDiceIndices(params int[] indices)
    {
        _keptIndices = indices;
    }

    private int _pickRuleIndex = 0;
    public void PickRuleIndex(int index)
    {
        _pickRuleIndex = index;
    }

    public Task StartGame(MultiScoreboard board)
    {
        StartGameWasCalled = true;
        return Task.CompletedTask;
    }

    public Task EndGame(MultiScoreboard board)
    {
        EndGameWasCalled = true;
        return Task.CompletedTask;
    }

    public Task<IList<DieRoll>> PickDiceToKeep(TurnState.RollTurnState state)
    {
        var allRolls = state.KeptDice.Concat(state.LastRoll).ToArray();
        return Task.FromResult((IList<DieRoll>)_keptIndices.Select(i => allRolls[i]).ToList());
    }

    public Task<RuleId> PickRuleToApply(TurnState.PickRuleTurnState state)
    {
        return Task.FromResult(state.SingleScoreboard.RulesWithScores.ElementAt(_pickRuleIndex).Id);
    }
}