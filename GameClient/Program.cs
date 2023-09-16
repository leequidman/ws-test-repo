using System.Net.WebSockets;
using System.Security.AccessControl;
using System.Text;
using Serilog;
using Serilog;
using System.ComponentModel;
using System.Text.Json;
using Common.Models.Requests.Login;
using Common.Models.Requests.UpdateResources;

namespace GameClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Hello, world!");


                var ws = new ClientWebSocket();
                while (true)
                {
                    Console.Write("Press any key to start");
                    Console.ReadKey();
                    break;
                }

                Log.Information("Start connecting...");
                await ws.ConnectAsync(new($"ws://localhost:13371/ws"), CancellationToken.None);
                Log.Information("Connected");


                var receiveTask = Task.Run(async () =>
                {
                    var buffer = new byte[1024 * 4];
                    while (true)
                    {
                        var result = await ws.ReceiveAsync(new(buffer), CancellationToken.None);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            break;
                        }

                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Log.Information(message);
                    }
                });

                var mdi = Guid.NewGuid();
                var sendTask = Task.Run(async () =>
                {
                    var continueLoop = true;
                    while (continueLoop)
                    {
                        var message = Console.ReadLine();

                        switch (message)
                        {
                            case "l":
                            {
                                var loginRequest = new InitLoginEvent(new(Guid.NewGuid()));

                                var bytes = JsonSerializer.SerializeToUtf8Bytes(loginRequest);
                                await ws.SendAsync(new(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                                break;
                            }
                            case "ll":
                            {
                                var loginRequest = new InitLoginEvent(new(mdi));

                                var bytes = JsonSerializer.SerializeToUtf8Bytes(loginRequest);
                                await ws.SendAsync(new(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                                break;
                            }
                            case "u":
                            {
                                var rnd = new Random();
                                var request = new UpdateResourceEvent(new(
                                    Common.Models.Requests.UpdateResources.ResourceType.Coins,
                                    rnd.Next(1, 10)));

                                var bytes = JsonSerializer.SerializeToUtf8Bytes(request);
                                await ws.SendAsync(new(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                                break;
                            }
                            case "e":
                            {
                                continueLoop = false;
                                break;
                            }
                            default:
                                Log.Warning($"Unknown command: '{message}'");
                                break;
                        }
                    }
                });

                await Task.WhenAny(sendTask, receiveTask);

                Log.Information("After when any");


                if (ws.State != WebSocketState.Closed)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }


                await Task.WhenAll(sendTask, receiveTask);
            }
            catch (Exception e)
            {
                Log.Error(e, "Unexpected error");
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }
    }
}