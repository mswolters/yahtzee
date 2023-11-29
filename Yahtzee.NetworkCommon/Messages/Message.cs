using System.Text.Json.Serialization;

namespace Yahtzee.NetworkCommon.Messages;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(NewPlayerMessage), "newPlayer")]
[JsonDerivedType(typeof(PlayerCreatedMessage), "playerCreated")]
[JsonDerivedType(typeof(PlayerPropertyChangedMessage), "playerPropertyChanged")]
public interface IMessage
{
    
}

public interface IClientMessage : IMessage
{
    
}

public interface IServerMessage : IMessage
{
    
}