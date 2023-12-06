using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Yahtzee.NetworkCommon.Messages;

namespace Yahtzee.Server;

public class ConnectedClient : INotifyConnectionClosed, INotifyMessageReceived
{
    private static readonly JsonSerializerOptions SerializeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    
    public int SocketId { get; }
    public WebSocket Socket { get; }
    public TaskCompletionSource<object> TaskCompletionSource { get; }

    public Channel<string> BroadcastQueue { get; } = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
        { SingleReader = true, SingleWriter = false });

    private CancellationTokenSource BroadcastLoopTokenSource { get; } = new CancellationTokenSource();

    public ConnectedClient(int socketId, WebSocket socket, TaskCompletionSource<object> taskCompletionSource)
    {
        SocketId = socketId;
        Socket = socket;
        TaskCompletionSource = taskCompletionSource;
    }

    public async Task Open(CancellationTokenSource socketLoopTokenSource)
    {
        _ = Task.Run(() => BroadcastLoopAsync().ConfigureAwait(false));

        var loopToken = socketLoopTokenSource.Token;
        try
        {
            var buffer = WebSocket.CreateServerBuffer(4096);
            while (Socket.State != WebSocketState.Closed && Socket.State != WebSocketState.Aborted &&
                   !loopToken.IsCancellationRequested)
            {
                var receiveResult = await Socket.ReceiveAsync(buffer, loopToken);
                // if the token is cancelled while ReceiveAsync is blocking, the socket state changes to aborted and it can't be used
                if (!loopToken.IsCancellationRequested)
                {
                    // the client is notifying us that the connection will close; send acknowledgement
                    if (Socket.State == WebSocketState.CloseReceived &&
                        receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine($"Socket {SocketId}: Acknowledging Close frame received from client");
                        BroadcastLoopTokenSource.Cancel();
                        await Socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Acknowledge Close frame",
                            CancellationToken.None);
                        // the socket state changes to closed at this point
                    }
                    
                    if (Socket.State == WebSocketState.Open)
                    {
                        OnMessageReceived(receiveResult, buffer);
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // normal upon task/token cancellation, disregard
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Socket {SocketId}: {ex}");
            //TODO handle exception
        }
        finally
        {
            BroadcastLoopTokenSource.Cancel();

            Console.WriteLine($"Socket {SocketId}: Ended processing loop in state {Socket.State}");

            // don't leave the socket in any potentially connected state
            if (Socket.State != WebSocketState.Closed)
                Socket.Abort();

            ConnectionClosed?.Invoke(this, new ConnectionClosedEventArgs(this));

            // signal to the middleware pipeline that this task has completed
            TaskCompletionSource.SetResult(true);
        }
    }

    private void OnMessageReceived(WebSocketReceiveResult receiveResult, ArraySegment<byte> buffer)
    {
        Console.WriteLine(
            $"Socket {SocketId}: Received {receiveResult.MessageType} frame ({receiveResult.Count} bytes).");
        try
        {
            var message = JsonSerializer.Deserialize<IMessage>(
                new ReadOnlySpan<byte>(buffer.Array, 0, receiveResult.Count), SerializeOptions)!;

            Console.WriteLine($"Socket {SocketId}: Received message: {message}");
            var handled = MessageReceived?.GetInvocationList()
                .Cast<MessageReceivedEventHandler>()
                .Aggregate(false, (current, handler) => current & handler(this, new MessageReceivedEventArgs(this, message))) ?? false;

            if (!handled)
            {
                BroadcastQueue.Writer.TryWrite($"Unhandled incoming message type: {message.GetType().Name}");
            }
        }
        catch (JsonException e)
        {
#if DEBUG
            BroadcastQueue.Writer.TryWrite($"Invalid message: {e.Message}");
#else
            BroadcastQueue.Writer.TryWrite("Invalid message");
#endif
            throw;
        }
    }

    public async Task Close()
    {
        BroadcastLoopTokenSource.Cancel();

        if (Socket.State != WebSocketState.Open)
        {
            Console.WriteLine($"... socket not open, state = {Socket.State}");
        }
        else
        {
            var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(120));
            try
            {
                Console.WriteLine("... starting close handshake");
                await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", timeout.Token);
            }
            catch (OperationCanceledException ex)
            {
                // normal upon task/token cancellation, disregard
            }
        }
    }

    private async Task BroadcastLoopAsync()
    {
        var cancellationToken = BroadcastLoopTokenSource.Token;
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                while (await BroadcastQueue.Reader.WaitToReadAsync(cancellationToken))
                {
                    string message = await BroadcastQueue.Reader.ReadAsync(cancellationToken);
                    Console.WriteLine($"Socket {SocketId}: Sending from queue.");
                    var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                    await Socket.SendAsync(buffer, WebSocketMessageType.Text, endOfMessage: true,
                        CancellationToken.None);
                }
            }
            catch (OperationCanceledException)
            {
                // normal upon task/token cancellation, disregard
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client {SocketId}: {ex}");
                // TODO nothing ever goes wrong.. right?
            }
        }
    }

    public event ConnectionClosedEventHandler? ConnectionClosed;
    public event MessageReceivedEventHandler? MessageReceived;
}