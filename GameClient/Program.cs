using System.Net.WebSockets;
using System.Text;

namespace GameClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var ws = new ClientWebSocket();
            Console.WriteLine("Start connecting...");
            await ws.ConnectAsync(new Uri("ws://localhost:13371/ws"), CancellationToken.None);
            Console.WriteLine("Connected");

            var receiveTask = Task.Run(async () =>
            {
                var buffer = new byte[1024];
                while (true)
                {
                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("Received close message");
                        break;
                    }

                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine("Received: " + message);
                }
            });

            await receiveTask;
        }
    }
}