namespace Yahtzee.Models;

public abstract record TurnState
{

    public static readonly RollTurnState StartOfTurn = new RollTurnState(0, new List<DieRoll>(), new List<DieRoll>());

    public record RollTurnState(int ThrowCount, IList<DieRoll> LastRoll, IList<DieRoll> KeptDice) : TurnState
    {
        public IEnumerable<DieRoll> AllDice => LastRoll.Concat(KeptDice);
    }

    public record PickRuleTurnState
        (
            SingleScoreboard SingleScoreboard, 
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