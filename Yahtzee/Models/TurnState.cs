namespace Yahtzee.Models;

internal abstract record TurnState
{

    public static readonly RollTurnState StartOfTurn = new RollTurnState(0, new List<DieRoll>(), new List<DieRoll>());

    internal record RollTurnState(int ThrowCount, IList<DieRoll> LastRoll, IList<DieRoll> KeptDice) : TurnState;

    internal record PickRuleTurnState(Scoreboard Scoreboard, IList<DieRoll> KeptDice) : TurnState;

}