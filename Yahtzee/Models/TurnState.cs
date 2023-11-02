namespace Yahtzee.Models;

internal abstract record TurnState
{

    public static readonly RollTurnState StartOfTurn = new RollTurnState(0, new List<DieRoll>(), new List<DieRoll>());

    internal record RollTurnState(int ThrowCount, IList<DieRoll> LastRoll, IList<DieRoll> KeptDice) : TurnState
    {
        public IEnumerable<DieRoll> AllDice => LastRoll.Concat(KeptDice);
    }

    internal record PickRuleTurnState
        (
            Scoreboard Scoreboard, 
            IList<DieRoll> KeptDice, 
            PickRuleTurnState.YahtzeeSpecialPickMode YahtzeeSpecialPick = PickRuleTurnState.YahtzeeSpecialPickMode.None
            ) : TurnState
    {
        public enum YahtzeeSpecialPickMode
        {
            None,
            Bottom,
            Top
        }
    }

}