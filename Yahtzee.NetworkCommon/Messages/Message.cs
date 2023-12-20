using System.Text.Json.Serialization;

namespace Yahtzee.NetworkCommon.Messages;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(NewPlayerMessage), "newPlayer")]
[JsonDerivedType(typeof(PlayerCreatedMessage), "playerCreated")]
[JsonDerivedType(typeof(PlayerPropertyChangedMessage), "playerPropertyChanged")]
[JsonDerivedType(typeof(StartGameMessage), "startGame")]
[JsonDerivedType(typeof(EndGameMessage), "endGame")]
[JsonDerivedType(typeof(PickDiceToKeepMessage), "pickDiceToKeep")]
[JsonDerivedType(typeof(PickDiceToKeepResponseMessage), "pickDiceToKeepResponse")]
[JsonDerivedType(typeof(PickRuleToApplyMessage), "pickRuleToApply")]
[JsonDerivedType(typeof(PickRuleToApplyResponseMessage), "pickRuleToApplyResponse")]
public interface IMessage
{
    
}

public interface IClientMessage : IMessage
{
    
}

public interface IServerMessage : IMessage
{
    
}