using Yahtzee.EventHandler;
using Yahtzee.Models;
using Yahtzee.Models.Rules;
using Yahtzee.Players;
using static Yahtzee.Models.TurnState.PickRuleTurnState.YahtzeeSpecialPickMode;

namespace Yahtzee;

public class GameManager : INotifyDiceRolled, INotifyDiceKept
{
    public event DiceRolledEventHandler? DiceRolled;
    public event DiceKeptEventHandler? DiceKept;

    public int ThrowsPerTurn { get; }
    public int DicePerThrow { get; }
    public int SidesPerDie { get; }

    public GameManager(int throwsPerTurn = 3, int dicePerThrow = 5, int sidesPerDie = 6)
    {
        ThrowsPerTurn = throwsPerTurn;
        DicePerThrow = dicePerThrow;
        SidesPerDie = sidesPerDie;
    }
    
    public bool HasEnded(MultiScoreboard board) => board[board.Players.Last()][Scorer.SumRuleId].Score.Written;

    public async Task RunGame(Random random, params IPlayablePlayer[] players)
    {
        var scoreboard = new MultiScoreboard(players.OfType<IPlayer>().ToList(), Scorer.DefaultBoard);
        Task.WaitAll(players.Select(player => player.StartGame(scoreboard)).ToArray());
        while (!HasEnded(scoreboard))
        {
            foreach (var player in players)
            {
                await DoTurn(scoreboard[player], player, random);
            }
        }
        Task.WaitAll(players.Select(player => player.EndGame(scoreboard)).ToArray());
    }
    
    internal async Task DoTurn(SingleScoreboard playerBoard, IPlayablePlayer player, Random random)
    {
        var turnState = TurnState.StartOfTurn;
        // The player gets 3 turns, but can also finish early by choosing to hold all dice
        while (turnState.ThrowCount < ThrowsPerTurn && turnState.KeptDice.Count < DicePerThrow)
        {
            turnState = await DoPartialTurn(turnState, player, random);
        }

        var yahtzeeSpecialCaseHasBeenApplied = await CheckAndExecuteYahtzeeSpecialCase(playerBoard, player, turnState.KeptDice);
        if (yahtzeeSpecialCaseHasBeenApplied)
        {
            return;
        }

        RuleId appliedRuleId;
        SingleScoreboard.RuleWithScore appliedRule;
        do
        {
            // Players are only allowed to pick rules which are playerWritable and which don't have a score yet.
            appliedRuleId = await player.PickRuleToApply(new TurnState.PickRuleTurnState(playerBoard, turnState.KeptDice));
            appliedRule = playerBoard[appliedRuleId];
        } while (!appliedRule.Rule.IsPlayerWritable || appliedRule.Score.Written);
        playerBoard.SetScore(appliedRuleId, appliedRule.Rule.Score(turnState.KeptDice, playerBoard));
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
    
    private async Task<bool> CheckAndExecuteYahtzeeSpecialCase(SingleScoreboard playerBoard, IPlayablePlayer player, IList<DieRoll> finalKeptDice)
    {
        var yahtzee = playerBoard[Scorer.YahtzeeRuleId];
        // No need to check anything when yahtzee isn't rolled
        if (yahtzee.Rule.Score(finalKeptDice, playerBoard).Value == 0) return false;
        // No special case when yahtzee hasn't been rolled before
        if (!yahtzee.Score.Written) return false;
        // No special case when yahtzee has been striked through
        if (yahtzee.Score.Value == 0) return false;
        
        // Woohoo, bonus!
        playerBoard.SetScore(Scorer.YahtzeeRuleId, yahtzee.Score with { Value = yahtzee.Score.Value + 100 });

        // Score the total of all 5 dice in the appropriate Upper Section box.

        var total = finalKeptDice.Sum(roll => roll.Value);
        var relevantTopRule = playerBoard[Scorer.TopRuleId(finalKeptDice[0])];
        if (!relevantTopRule.Score.Written)
        {
            playerBoard.SetScore(relevantTopRule.Id, new Score(total));
            return true;
        }
        
        // If this box has already been filled in, score as follows in any open Lower Section box:
        // 3/4 of a kind: total of all 5 dice
        // Full House: 25
        // Small Straight: 30
        // Large Straight: 40
        // Chance: total of all 5 dice

        // If the appropriate Upper Section box and all Lower Section boxes are
        // all filled in, you must enter a zero in any Upper Section box.
        
        var pickMode = playerBoard[Scorer.BottomSumRuleId].Score.Written ? Top : Bottom;

        RuleId appliedRuleId;
        SingleScoreboard.RuleWithScore appliedRule;
        do
        {
            // Players are only allowed to pick rules which are playerWritable and which don't have a score yet.
            appliedRuleId = await player.PickRuleToApply(new TurnState.PickRuleTurnState(playerBoard, finalKeptDice, pickMode));
            appliedRule = playerBoard[appliedRuleId];
        } while (!appliedRule.Rule.IsPlayerWritable || appliedRule.Score.Written || !RuleFitsPickMode(playerBoard, appliedRule, pickMode));

        if (pickMode == Top)
        {
            playerBoard.SetScore(appliedRuleId, new Score(0));
        }
        else
        {
            int scoreValue;
            if (appliedRuleId == Scorer.FullHouseRuleId) scoreValue = 25;
            else if (appliedRuleId == Scorer.StraightRuleId(4)) scoreValue = 30;
            else if (appliedRuleId == Scorer.StraightRuleId(5)) scoreValue = 40;
            else scoreValue = total;
            playerBoard.SetScore(appliedRuleId, new Score(scoreValue));
        }

        return true;

    }

    private bool RuleFitsPickMode(SingleScoreboard playerBoard, SingleScoreboard.RuleWithScore rule, TurnState.PickRuleTurnState.YahtzeeSpecialPickMode pickMode)
    {
        return pickMode switch
        {
            None => true,
            Bottom => ((IDependOnRules)playerBoard[Scorer.BottomSumRuleId].Rule).DependsOnIds.Contains(rule.Id),
            Top => ((IDependOnRules)playerBoard[Scorer.TopSubSumRuleId].Rule).DependsOnIds.Contains(rule.Id),
            _ => throw new ArgumentOutOfRangeException(nameof(pickMode), pickMode, null)
        };
    }

}