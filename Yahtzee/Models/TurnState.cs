namespace Yahtzee.Models;

internal record TurnState(int ThrowCount, IList<DieRoll> KeptDice)
{

    public static readonly TurnState StartOfTurn = new(0, new List<DieRoll>());

}