using Yahtzee.Models;
using Yahtzee.Tests.Mock;

namespace Yahtzee.Tests;

[TestClass]
public class TurnTests
{
    private MockRandom _random = new();
    private GameState _gameState = new(3, 5, 6);
    private MockPlayer _player = new();

    [TestInitialize]
    public void Setup()
    {
        _random = new MockRandom();
        _gameState = new GameState();
        _player = new MockPlayer();
    }

    [TestMethod]
    public void CorrectNumberOfDiceOnFirstRollTest()
    {
        _random.MockNext(0, 0, 0, 0, 0);
        var roll = _gameState.DoRoll(
            new TurnState.RollTurnState(0, DiceTestsHelpers.RollsOf(), DiceTestsHelpers.RollsOf()), _random);
        YahtzeeAssert.AreEqual(DiceTestsHelpers.RollsOf(1, 1, 1, 1, 1), roll);
    }

    [TestMethod]
    public void CorrectNumberOfDiceOnConsecutiveRollTest()
    {
        _random.MockNext(0, 0, 0, 0, 0);
        var roll = _gameState.DoRoll(
            new TurnState.RollTurnState(1, DiceTestsHelpers.RollsOf(), DiceTestsHelpers.RollsOf(1, 2, 3)), _random);
        YahtzeeAssert.AreEqual(DiceTestsHelpers.RollsOf(1, 1), roll);
    }

    [TestMethod]
    public async Task PartialTurnTest()
    {
        _random.MockNext(0, 2, 1, 3, 0);
        _player.KeepDiceIndices(1, 2, 3);
        var resultingState = await _gameState.DoPartialTurn(
            new TurnState.RollTurnState(0, DiceTestsHelpers.RollsOf(), DiceTestsHelpers.RollsOf()), _player, _random);
        Assert.AreEqual(1, resultingState.ThrowCount);
        YahtzeeAssert.AreEqual(DiceTestsHelpers.RollsOf(3, 2, 4), resultingState.KeptDice);
    }

    [TestMethod]
    public async Task PartialTurnKeepsAllDiceOnFinalTurnTest()
    {
        _random.MockNext(0, 1, 2, 3, 4);
        _player.KeepDiceIndices(0, 1);
        var resultingState =
            await _gameState.DoPartialTurn(new TurnState.RollTurnState(2, DiceTestsHelpers.RollsOf(), DiceTestsHelpers.RollsOf()), _player, _random);
        Assert.AreEqual(3, resultingState.ThrowCount);
        YahtzeeAssert.AreEqual(DiceTestsHelpers.RollsOf(1, 2, 3, 4, 5), resultingState.KeptDice);
    }
}