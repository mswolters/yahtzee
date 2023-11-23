using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace Yahtzee.Server;

public class WebSocketMiddleware : IMiddleware
{
    private static int SocketCounter = 0;

    // The key is a socket id
    private static ConcurrentDictionary<int, ConnectedClient>
        Clients = new ConcurrentDictionary<int, ConnectedClient>();

    private static readonly CancellationTokenSource SocketLoopTokenSource = new();

    private static bool _serverIsRunning = true;

    private static CancellationTokenRegistration _appShutdownHandler;

    // use dependency injection to grab a reference to the hosting container's lifetime cancellation tokens
    public WebSocketMiddleware(IHostApplicationLifetime hostLifetime)
    {
        // gracefully close all websockets during shutdown (only register on first instantiation)
        if (_appShutdownHandler.Token.Equals(CancellationToken.None))
            _appShutdownHandler = hostLifetime.ApplicationStopping.Register(ApplicationShutdownHandler);
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            if (_serverIsRunning)
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var socketId = Interlocked.Increment(ref SocketCounter);
                    var socket = await context.WebSockets.AcceptWebSocketAsync();
                    var completion = new TaskCompletionSource<object>();
                    var client = new ConnectedClient(socketId, socket, completion);
                    client.ConnectionClosed += OnConnectionClosed;
                    Clients.TryAdd(socketId, client);
                    Console.WriteLine($"Socket {socketId}: New connection.");

                    // TaskCompletionSource<> is used to keep the middleware pipeline alive;
                    // SocketProcessingLoop calls TrySetResult upon socket termination
                    _ = Task.Run(() => client.Open(SocketLoopTokenSource));
                    await completion.Task;
                }
                else
                {
                    if (context.Request.Headers["Accept"][0]?.Contains("text/html") == true)
                    {
                        Console.WriteLine("Sending HTML to client.");
                        await context.Response.WriteAsync("" /*SimpleHtmlClient.HTML*/);
                    }
                    else
                    {
                        // ignore other requests (such as favicon)
                        // potentially other middleware will handle it (see finally block)
                    }
                }
            }
            else
            {
                // ServerIsRunning = false
                // HTTP 409 Conflict (with server's current state)
                context.Response.StatusCode = 409;
            }
        }
        catch (Exception ex)
        {
            // HTTP 500 Internal server error
            context.Response.StatusCode = 500;
            Console.WriteLine(ex);
        }
        finally
        {
            // if this middleware didn't handle the request, pass it on
            if (!context.Response.HasStarted)
                await next(context);
        }
    }

    public void OnConnectionClosed(object sender, ConnectionClosedEventArgs e)
    {
        // by this point the socket is closed or aborted, the ConnectedClient object is useless
        if (Clients.TryRemove(e.Client.SocketId, out _))
            e.Client.Socket.Dispose();
    }

    // event-handlers are the sole case where async void is valid
    public static async void ApplicationShutdownHandler()
    {
        _serverIsRunning = false;
        await CloseAllSocketsAsync();
    }

    private static async Task CloseAllSocketsAsync()
    {
        // We can't dispose the sockets until the processing loops are terminated,
        // but terminating the loops will abort the sockets, preventing graceful closing.
        var disposeQueue = new List<WebSocket>(Clients.Count);

        while (!Clients.IsEmpty)
        {
            var client = Clients.ElementAt(0).Value;
            Console.WriteLine($"Closing Socket {client.SocketId}");

            Console.WriteLine("... ending broadcast loop");
            await client.Close();

            if (Clients.TryRemove(client.SocketId, out _))
            {
                // only safe to Dispose once, so only add it if this loop can't process it again
                disposeQueue.Add(client.Socket);
            }

            Console.WriteLine("... done");
        }

        // now that they're all closed, terminate the blocking ReceiveAsync calls in the SocketProcessingLoop threads
        SocketLoopTokenSource.Cancel();

        // dispose all resources
        foreach (var socket in disposeQueue)
            socket.Dispose();
    }
}