﻿using System.ComponentModel;
using Yahtzee.Models;
using Yahtzee.Models.Rules;

namespace Yahtzee.Tests.Mock;

internal class MockPlayer : IPlayablePlayer
{
    public string Name { get => "MockPlayer"; set => throw new NotImplementedException(); }

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

    public Task<IList<DieRoll>> PickDiceToKeep(TurnState.RollTurnState state)
    {
        var allRolls = state.KeptDice.Concat(state.LastRoll).ToArray();
        return Task.FromResult((IList<DieRoll>)_keptIndices.Select(i => allRolls[i]).ToList());
    }

    public Task<Rule> PickRuleToApply(TurnState.PickRuleTurnState state)
    {
        return Task.FromResult(state.Scoreboard.Rules.ElementAt(_pickRuleIndex));
    }
}