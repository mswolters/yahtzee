using Yahtzee.Models;
using Yahtzee.Models.Rules;

namespace Yahtzee.NetworkCommon.Messages;

public class StartGameMessage : IServerMessage
{
    public MultiScoreboard Scoreboard { get; }

    public StartGameMessage(MultiScoreboard scoreboard)
    {
        Scoreboard = scoreboard;
    }
}

public class EndGameMessage : IServerMessage
{
    public MultiScoreboard Scoreboard { get; }

    public EndGameMessage(MultiScoreboard scoreboard)
    {
        Scoreboard = scoreboard;
    }
}

public class PickDiceToKeepMessage : IServerMessage
{
    public TurnState.RollTurnState State { get; }

    public PickDiceToKeepMessage(TurnState.RollTurnState state)
    {
        State = state;
    }
}

public class PickDiceToKeepResponseMessage : IClientMessage
{
    public IList<DieRoll> DiceToKeep { get; }

    public PickDiceToKeepResponseMessage(IList<DieRoll> diceToKeep)
    {
        DiceToKeep = diceToKeep;
    }
}

public class PickRuleToApplyMessage : IServerMessage
{
    public TurnState.PickRuleTurnState State { get; }

    public PickRuleToApplyMessage(TurnState.PickRuleTurnState state)
    {
        State = state;
    }
}

public class PickRuleToApplyResponseMessage : IClientMessage
{
    public RuleId RuleId { get; }

    public PickRuleToApplyResponseMessage(RuleId ruleId)
    {
        RuleId = ruleId;
    }
}