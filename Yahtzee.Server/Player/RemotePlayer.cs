using System.ComponentModel;
using Yahtzee.Models;
using Yahtzee.Models.Rules;
using Yahtzee.NetworkCommon.Messages;
using Yahtzee.Players;

namespace Yahtzee.Server.Player;

public class RemotePlayer : IPlayablePlayer
{
    public RemotePlayer(string name, ConnectedClient client)
    {
        Name = name;
        Client = client;
        PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(Name))
            {
                Client.SendMessage(new PlayerPropertyChangedMessage(this, nameof(Name)));
            }
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; }
    public ConnectedClient Client { get; }

    public Task StartGame(MultiScoreboard board)
    {
        Client.SendMessage(new StartGameMessage(board));
        return Task.CompletedTask;
    }

    public Task EndGame(MultiScoreboard board)
    {
        Client.SendMessage(new EndGameMessage(board));
        return Task.CompletedTask;
    }

    public async Task<IList<DieRoll>> PickDiceToKeep(TurnState.RollTurnState state)
    {
        Client.SendMessage(new PickDiceToKeepMessage(state));
        var response = await Client.ReceiveMessage<PickDiceToKeepResponseMessage>();
        return response.DiceToKeep;
    }

    public async Task<RuleId> PickRuleToApply(TurnState.PickRuleTurnState state)
    {
        Client.SendMessage(new PickRuleToApplyMessage(state));
        var response = await Client.ReceiveMessage<PickRuleToApplyResponseMessage>();
        return response.RuleId;
    }
}