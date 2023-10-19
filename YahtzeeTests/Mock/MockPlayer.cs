using System.ComponentModel;
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

    public Task<IList<DieRoll>> PickDiceToKeep(TurnState state, IList<DieRoll> rolls)
    {
        var allRolls = state.KeptDice.Concat(rolls).ToArray();
        return Task.FromResult((IList<DieRoll>)_keptIndices.Select(i => allRolls[i]).ToList());
    }

    public Task<Rule> PickRuleToApply(Scoreboard board, IList<DieRoll> rolls)
    {
        return Task.FromResult(board.Rules.ElementAt(_pickRuleIndex));
    }
}