namespace Yahtzee.Server;

public interface INotifyConnectionClosed
{
    event ConnectionClosedEventHandler? ConnectionClosed;
}

public delegate void ConnectionClosedEventHandler(object sender, ConnectionClosedEventArgs e);

public class ConnectionClosedEventArgs : EventArgs
{
    public ConnectionClosedEventArgs(ConnectedClient client)
    {
        Client = client;
    }

    public ConnectedClient Client { get; }
}