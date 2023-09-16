using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Common.Models.Response.Abstract;

namespace GameServer.Client
{
    public interface ISender
    {
        Task Send(WebSocket ws, IResponse response);
        Task Send(WebSocket ws, byte[] data);
    }

    public class Sender : ISender
    {
        public async Task Send(WebSocket ws, IResponse response)
        {
            var data = JsonSerializer.SerializeToUtf8Bytes(response);
            await Send(ws, data);
        }
        public async Task Send(WebSocket ws, byte[] data)
        {
            var arraySegment = new ArraySegment<byte>(data, 0, data.Length);
            await ws.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
