using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace GameServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("http://localhost:1337");
            var app = builder.Build();

            app.UseWebSockets();

            app.Map("/ws",
                async context =>
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using var ws = await context.WebSockets.AcceptWebSocketAsync();

                        while (true)
                        {
                            var message = "Time now is: " + DateTime.Now.ToString("HH:mm:ss");
                            var bytes = Encoding.UTF8.GetBytes(message);
                            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                            if (ws.State == WebSocketState.Open)
                            {
                                await ws.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                            else if (ws.State == WebSocketState.Closed || ws.State == WebSocketState.Aborted)
                            {
                                break;
                            }

                            await Task.Delay(1000);
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                });
            await app.RunAsync();
        }
    }
}