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
    public IPlayer Player { get; }
    public string ChangedProperty { get; }
    
    public PlayerPropertyChangedMessage(IPlayer player, string changedProperty)
    {
        Player = player;
        ChangedProperty = changedProperty;
    }
}