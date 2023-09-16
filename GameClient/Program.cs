using System.Net.WebSockets;
using System.Text;
using Serilog;
using Serilog;

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
                Console.WriteLine("Hello, World!");
                Log.Information("Hello, world!");


                var ws = new ClientWebSocket();
                string name;
                while (true)
                {
                    Console.Write("Input name: ");
                    name = Console.ReadLine();
                    break;
                }

                Console.WriteLine("Start connecting...");
                await ws.ConnectAsync(new Uri($"ws://localhost:13371/ws?name={name}"), CancellationToken.None);
                Console.WriteLine("Connected");


                var receiveTask = Task.Run(async () =>
                {
                    var buffer = new byte[1024 * 4];
                    while (true)
                    {
                        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            break;
                        }

                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine(message);
                    }
                });

                var sendTask = Task.Run(async () =>
                {
                    while (true)
                    {
                        var message = Console.ReadLine();

                        if (message == "exit")
                        {
                            break;
                        }

                        var bytes = Encoding.UTF8.GetBytes(message);
                        await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                });

                await Task.WhenAny(sendTask, receiveTask);

                Console.WriteLine("After when any");


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







            // var ws = new ClientWebSocket();
            // Console.WriteLine("Start connecting...");
            // await ws.ConnectAsync(new Uri("ws://localhost:13371/ws"), CancellationToken.None);
            // Console.WriteLine("Connected");
            //
            // var receiveTask = Task.Run(async () =>
            // {
            //     var buffer = new byte[1024];
            //     while (true)
            //     {
            //         var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            //         if (result.MessageType == WebSocketMessageType.Close)
            //         {
            //             Console.WriteLine("Received close message");
            //             break;
            //         }
            //
            //         var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            //         Console.WriteLine("Received: " + message);
            //     }
            // });
            //
            // await receiveTask;
        }
    }
}