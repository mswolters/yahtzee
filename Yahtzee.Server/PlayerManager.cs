using Yahtzee.NetworkCommon.Messages;
using Yahtzee.NetworkCommon.Models;
using Yahtzee.Server.Player;

namespace Yahtzee.Server;

public class PlayerManager
{
    public static readonly PlayerManager Instance = new();

    public IList<RemotePlayer> Players { get; } = new List<RemotePlayer>();

    public async void OnClientConnected(ConnectedClient client)
    {
        client.ConnectionClosed += OnClientDisconnected;
        try
        {
            var message = await client.ReceiveMessage<NewPlayerMessage>();
            await OnNewPlayerMessage(client, message);
        }
        catch (OperationCanceledException)
        {
            // Normal exception, ignore
        }
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
        if (Players.Any(p => p.Client == client))
        {
            return;
        }
        var player = new RemotePlayer(message.Name, client);
        Players.Add(player);
        player.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(player.Name))
            {
                Players.Select(p => p.Client).ToList().ForEach(c =>
                    c.SendMessage(new PlayerPropertyChangedMessage(SimplePlayer.Of(player), nameof(player.Name))));
            }
        };
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