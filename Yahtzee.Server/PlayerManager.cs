using Yahtzee.NetworkCommon.Messages;
using Yahtzee.Players;
using Yahtzee.Players.ConsolePlayer;
using Yahtzee.Server.Player;

namespace Yahtzee.Server;

public class PlayerManager
{
    public static readonly PlayerManager Instance = new();
    
    public IList<RemotePlayer> Players { get; } = new List<RemotePlayer>();
    
    public async void OnClientConnected(ConnectedClient client)
    {
        client.ConnectionClosed += OnClientDisconnected;
        var message = await client.ReceiveMessage<NewPlayerMessage>();
        await OnNewPlayerMessage(client, message);
    }
    
    private void OnClientDisconnected(object? sender, ConnectionClosedEventArgs e)
    {
        var client = e.Client;
        client.ConnectionClosed -= OnClientDisconnected;
        var player = Players.FirstOrDefault(p => p.Client == client);
        if (player != null)
        {
            Players.Remove(player);
        }
    }
    
    private async Task OnNewPlayerMessage(ConnectedClient client, NewPlayerMessage message)
    {
        var player = new RemotePlayer(message.Name, client);
        Players.Add(player);
        client.SendMessage(new PlayerCreatedMessage(player));
        
        // Just start a game with 1 player for now
        
        var state = new GameManager();
        var random = new Random();

        try
        {
            await state.RunGame(random, player);
        }
        catch (OperationCanceledException)
        {
            // Normal exception, ignore
        }
    }
}