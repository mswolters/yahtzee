using Yahtzee.EventHandler;
using Yahtzee.Models;
using Yahtzee.Models.Rules;
using Yahtzee.Players;

namespace Yahtzee;

internal class GameState : INotifyDiceRolled, INotifyDiceKept
{
    public Scoreboard Scoreboard { get; } = Scorer.DefaultBoard;

    public event DiceRolledEventHandler? DiceRolled;
    public event DiceKeptEventHandler? DiceKept;

    public int ThrowsPerTurn { get; }
    public int DicePerThrow { get; }
    public int SidesPerDie { get; }
    
    public bool HasEnded => Scoreboard.RulesWithScores.Last().Score.Written;

    public GameState(int throwsPerTurn = 3, int dicePerThrow = 5, int sidesPerDie = 6)
    {
        ThrowsPerTurn = throwsPerTurn;
        DicePerThrow = dicePerThrow;
        SidesPerDie = sidesPerDie;
    }

    internal async Task RunGame(IPlayablePlayer player, Random random)
    {
        await player.StartGame(this);
        while (!HasEnded)
        {
            await DoTurn(player, random);
        }
        await player.EndGame(this);
    }
    
    internal async Task DoTurn(IPlayablePlayer player, Random random)
    {
        var turnState = TurnState.StartOfTurn;
        // The player gets 3 turns, but can also finish early by choosing to hold all dice
        while (turnState.ThrowCount < ThrowsPerTurn && turnState.KeptDice.Count < DicePerThrow)
        {
            turnState = await DoPartialTurn(turnState, player, random);
        }

        IRule appliedRule;
        do
        {
            // Players are only allowed to pick rules which are playerWritable and which don't have a score yet.
            appliedRule = await player.PickRuleToApply(new TurnState.PickRuleTurnState(Scoreboard, turnState.KeptDice));
        } while (!appliedRule.IsPlayerWritable || Scoreboard[appliedRule].Written);
        Scoreboard.SetScore(appliedRule, appliedRule.Score(turnState.KeptDice, Scoreboard));
    }

    internal async Task<TurnState.RollTurnState> DoPartialTurn(TurnState.RollTurnState turnState, IPlayablePlayer player, Random random)
    {
        var rolledDice = DoRoll(turnState, random);
        DiceRolled?.Invoke(this, new DiceRolledEventArgs(player, rolledDice));
        var stateVisibleToPlayer =
            new TurnState.RollTurnState(turnState.ThrowCount + 1, rolledDice, turnState.KeptDice);
        IList<DieRoll> heldDice;
        if (turnState.ThrowCount >= ThrowsPerTurn - 1)
        {
            // All dice must be used after the third roll
            heldDice = turnState.KeptDice.Concat(rolledDice).ToList();
        }
        else
        {
            // The player gets to choose which dice to keep in any other case
            do
            {
                heldDice = await player.PickDiceToKeep(stateVisibleToPlayer);
            } while (!stateVisibleToPlayer.AllDice.ContainsAll(heldDice));
        }
        DiceKept?.Invoke(this, new DiceRolledEventArgs(player, heldDice));
        return new TurnState.RollTurnState(stateVisibleToPlayer.ThrowCount, stateVisibleToPlayer.LastRoll, heldDice);
    }

    internal IList<DieRoll> DoRoll(TurnState.RollTurnState turnState, Random random)
    {
        var diceToThrow = DicePerThrow - turnState.KeptDice.Count;
        return Roll(Enumerable.Repeat(new Die(SidesPerDie), diceToThrow), random);
    }

    private static IList<DieRoll> Roll(IEnumerable<Die> dice, Random random)
    {
        // We need to evaluate the enumerable by calling ToList, or it will keep giving different results every time it's evaluated
        return dice.Select(die => die.Roll(random)).ToList();
    }

}