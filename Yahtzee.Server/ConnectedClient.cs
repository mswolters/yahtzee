using System.Net.WebSockets;
using System.Text;
using System.Threading.Channels;

namespace Yahtzee.Server;

public class ConnectedClient : INotifyConnectionClosed
{
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


                    // echo text or binary data to the broadcast queue
                    if (Socket.State == WebSocketState.Open)
                    {
                        Console.WriteLine(
                            $"Socket {SocketId}: Received {receiveResult.MessageType} frame ({receiveResult.Count} bytes).");
                        string message = Encoding.UTF8.GetString(buffer.Array!, buffer.Offset, receiveResult.Count);
                        // TODO handle message
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
}