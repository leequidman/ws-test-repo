using System.Net;
using System.Net.WebSockets;
using System.Text;
using Serilog;

namespace GameServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            ConfigureLogger();

            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("http://localhost:13371");
            builder.Host.UseSerilog();

            var app = builder.Build();

            app.UseWebSockets();

            var connections = new List<WebSocket>();

            app.Map("/ws", async context =>
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using var ws = await context.WebSockets.AcceptWebSocketAsync();
                        connections.Add(ws);

                        var curName = context.Request.Query["name"];
                        await Broadcast($"{curName} joined the room");
                        await Broadcast($"{connections.Count} users connected");

                        await ReceiveMessage(ws,
                            async (result, buffer) =>
                            {
                                if (result.MessageType == WebSocketMessageType.Text)
                                {
                                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                                    await Broadcast(curName + ": " + message);
                                }
                                else if (result.MessageType == WebSocketMessageType.Close || ws.State == WebSocketState.Aborted)
                                {
                                    connections.Remove(ws);
                                    await Broadcast($"{curName} left the room");
                                    await Broadcast($"{connections.Count} users connected");
                                    await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                                }
                            });

                        

                        async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
                        {
                            var buffer = new byte[1024 * 4];
                            while (socket.State == WebSocketState.Open)
                            {
                                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                                handleMessage(result, buffer);
                            }
                        }

                        async Task Broadcast(string message)
                        {
                            var bytes = Encoding.UTF8.GetBytes(message);
                            foreach (var socket in connections)
                            {
                                if (socket.State == WebSocketState.Open)
                                {
                                    var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                                    await socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                                }
                            }
                        }

                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                });


            await app.RunAsync();


        }
        private static void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }
    }





}//     // //     while (true)
//     // //     {
//     // //         var message = "Time now is: " + DateTime.Now.ToString("HH:mm:ss");
//     // //         var bytes = Encoding.UTF8.GetBytes(message);
//     // //         var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
//     // //         if (ws.State == WebSocketState.Open)
//     // //         {
//     // //             Console.WriteLine("Sending this: " + message);
//     // //             await ws.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
//     // //         }
//     // //         else if (ws.State == WebSocketState.Closed || ws.State == WebSocketState.Aborted)
//     // //         {
//     // //             Console.WriteLine("Websocket closed. By client?");  
//     // //             break;
//     // //         }
//     // //
//     // //         await Task.Delay(1000);
//     // //     }
//     // // }
//     // else
//     // {
//     //     context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
//     // }
// });