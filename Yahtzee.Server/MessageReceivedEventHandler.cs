using Yahtzee.NetworkCommon.Messages;

namespace Yahtzee.Server;

public interface INotifyMessageReceived
{
    event MessageReceivedEventHandler? MessageReceived;
}

public delegate bool MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

public interface IMessageReceivedEventHandler<in T> where T : IMessage
{
    bool OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        if (e.Message is T message)
        {
            return HandleMessage(e.Client, message);
        }

        return false;
    }

    bool HandleMessage(ConnectedClient client, T message);
}

public class MessageReceivedEventArgs : EventArgs
{
    public MessageReceivedEventArgs(ConnectedClient client, IMessage message)
    {
        Client = client;
        Message = message;
    }

    public ConnectedClient Client { get; }
    public IMessage Message { get; }
}