using Yahtzee.EventHandler;
using Yahtzee.Models;

namespace Yahtzee;

internal class GameState : INotifyDiceRolled, INotifyDiceKept
{
    public Scoreboard _scoreboard = Scorer.DefaultBoard;

    public event DiceRolledEventHandler? DiceRolled;
    public event DiceKeptEventHandler? DiceKept;

    public int ThrowsPerTurn { get; }
    public int DicePerThrow { get; }
    public int SidesPerDie { get; }

    public GameState(int throwsPerTurn = 3, int dicePerThrow = 5, int sidesPerDie = 6)
    {
        ThrowsPerTurn = throwsPerTurn;
        DicePerThrow = dicePerThrow;
        SidesPerDie = sidesPerDie;
    }

    internal async Task DoTurn(IPlayablePlayer player, Random random)
    {
        var turnState = TurnState.StartOfTurn;
        // The player gets 3 turns, but can also finish early by choosing to hold all dice
        while (turnState.ThrowCount < ThrowsPerTurn && turnState.KeptDice.Count < DicePerThrow)
        {
            turnState = await DoPartialTurn(turnState, player, random);
        }

        var appliedRule = await player.PickRuleToApply(_scoreboard, turnState.KeptDice);
        _scoreboard.SetScore(appliedRule, appliedRule.Score(turnState.KeptDice, _scoreboard));
    }

    internal async Task<TurnState> DoPartialTurn(TurnState turnState, IPlayablePlayer player, Random random)
    {
        var rolledDice = DoRoll(turnState, random);
        DiceRolled?.Invoke(this, new(player, rolledDice));
        IList<DieRoll> heldDice;
        if (turnState.ThrowCount >= ThrowsPerTurn - 1)
        {
            // All dice must be used after the third roll
            heldDice = turnState.KeptDice.Concat(rolledDice).ToList();
        }
        else
        {
            // The player gets to choose which dice to keep in any other case
            heldDice = await player.PickDiceToKeep(turnState, rolledDice);
        }
        DiceKept?.Invoke(this, new(player, heldDice));
        return new TurnState(turnState.ThrowCount + 1, heldDice);
    }

    internal IList<DieRoll> DoRoll(TurnState turnState, Random random)
    {
        var diceToThrow = DicePerThrow - turnState.KeptDice.Count();
        return Roll(Enumerable.Repeat(new Die(SidesPerDie), diceToThrow), random);
    }

    private static IList<DieRoll> Roll(IEnumerable<Die> dice, Random random)
    {
        // We need to evaluate the enumerable by calling ToList, or it will keep giving different results every time it's evaluated
        return dice.Select(die => die.Roll(random)).ToList();
    }

}