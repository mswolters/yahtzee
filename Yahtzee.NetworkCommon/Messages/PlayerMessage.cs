using Yahtzee.NetworkCommon.Models;
using Yahtzee.Players;

namespace Yahtzee.NetworkCommon.Messages;

public class NewPlayerMessage : IClientMessage
{
    public NewPlayerMessage(string name)
    {
        Name = name;
    }

    public string Name { get; }
}

public class PlayerCreatedMessage : IServerMessage
{
    public IPlayer Player { get; }

    public PlayerCreatedMessage(IPlayer player)
    {
        Player = player;
    }
}

public class PlayerPropertyChangedMessage : IServerMessage, IClientMessage
{
    public SimplePlayer Player { get; }
    public string ChangedProperty { get; }
    
    public PlayerPropertyChangedMessage(SimplePlayer player, string changedProperty)
    {
        Player = player;
        ChangedProperty = changedProperty;
    }
}