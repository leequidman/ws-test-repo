using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Nodes;
using Common.Models;
using Common.Models.Requests.Abstract;
using Common.Models.Requests.Login;
using Common.Models.Requests.UpdateResources;
using Common.Services;
using GameServer.Handlers;
using GameServer.Repositories;
using GameServer.Services;
using Serilog;

namespace GameServer
{

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var log = Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);
                builder.WebHost.UseUrls("http://localhost:13371");
                builder.Host.UseSerilog();

                builder.Services.AddSingleton<Serilog.ILogger>(log);
                builder.Services.AddSingleton<ILoginHandler, LoginHandler>();
                builder.Services.AddSingleton<IPlayersService, PlayersService>();
                builder.Services.AddSingleton<IConnectionService, ConnectionService>();
                builder.Services.AddSingleton<IPlayersRepository, PlayersRepository>();
                builder.Services.AddSingleton<IEventSender, EventSender>();



                var app = builder.Build();

                app.UseWebSockets();

                var connections = new List<WebSocket>();

                app.Map("/ws",
                    async context =>
                    {
                        if (context.WebSockets.IsWebSocketRequest)
                        {
                            using var ws = await context.WebSockets.AcceptWebSocketAsync();
                            connections.Add(ws);

                            await ReceiveMessage(ws,
                                async (result, buffer) =>
                                {
                                    if (result.MessageType == WebSocketMessageType.Text)
                                    {
                                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                                        IEvent request = null;
                                        try
                                        {
                                            request = Parse(message);
                                        }
                                        catch (Exception e)
                                        {
                                            var bytes = Encoding.UTF8.GetBytes(e.Message);
                                            if (ws.State == WebSocketState.Open)
                                            {
                                                var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                                                await ws.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                                                return;
                                            }
                                        }

                                        // var request = JsonSerializer.Deserialize<Request>(message);
                                        switch (request.EventType)
                                        {
                                            case EventType.InitLogin:
                                                var h = app.Services.GetService<ILoginHandler>();
                                                
                                                var data = request.EventData as InitLoginEventData;

                                                await h.Handle(data.DeviceId, ws);

                                                // await HandleLogin(ws, request.EventData);
                                                break;
                                            case EventType.UpdateResources:
                                                await HandleUpdateResources(ws, request.EventData);
                                                break;
                                            case EventType.SendGift:
                                                break;
                                            default:
                                                throw new ArgumentOutOfRangeException();
                                        }


                                        // handlerFactory.TryGetHandler(request.RequestType, out var handler);
                                        // var res = await handler.Handle(request.RequestData);


                                        // await Broadcast(curName + ": " + message);
                                    }
                                    else if (result.MessageType == WebSocketMessageType.Close || ws.State == WebSocketState.Aborted)
                                    {
                                        connections.Remove(ws);
                                        // await Broadcast($"{curName} left the room");
                                        // await Broadcast($"{connections.Count} users connected");
                                        await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                                    }
                                });


                            async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
                            {
                                var buffer = new byte[1024 * 4];
                                while (socket.State == WebSocketState.Open)
                                {
                                    var result = await socket.ReceiveAsync(new(buffer), CancellationToken.None);
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
            catch (Exception e)
            {
                Log.Fatal(e, "Unexpected error");
                throw;
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }

        private static IEvent Parse(string jsonString)
        {
            var jsonObj = JsonNode.Parse(jsonString)?.AsObject();
            Enum.TryParse<EventType>(jsonObj["EventType"].ToString(), out var type);

            switch (type)
            {
                case EventType.InitLogin:
                    return new InitLoginEvent(new(Guid.Parse(jsonObj["EventData"]["DeviceId"].ToString())));

                case EventType.UpdateResources:
                    return new UpdateResourceEvent(new(
                        Enum.Parse<ResourceType>(jsonObj["EventData"]["ResourceType"].ToString()),
                        int.Parse(jsonObj["EventData"]["Amount"].ToString())));

                // case EventType.SendGift:
                //     break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static async Task HandleUpdateResources(WebSocket ws, object eventEventData)
        {
            var data = eventEventData as UpdateResourcesEventData;
            Log.Information($"Handling UPDATE, Updating '{data.Amount}' of '{data.ResourceType}");
            await Task.Delay(500);
            Log.Information($"UPDATED");
            ;

            var bytes = Encoding.UTF8.GetBytes($"Now your '{data.ResourceType}' is  '{data.Amount}'");
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
            await ws.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private static async Task HandleLogin(WebSocket ws, object requestRequestData)
        {
            
            var data = requestRequestData as InitLoginEventData;
            Log.Information($"Handling login, DeviceId: {data.DeviceId}");
            await Task.Delay(500);
            Log.Information($"Logged in, DeviceId: {data.DeviceId}");

            var bytes = Encoding.UTF8.GetBytes($"Hi, {data.DeviceId}, you are logged in");
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
            await ws.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);

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