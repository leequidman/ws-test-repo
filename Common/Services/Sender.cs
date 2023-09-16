using System.Net.WebSockets;
using System.Text.Json;
using Common.Models.Requests.Abstract;

namespace Common.Services
{
    public interface IEventSender
    {
        Task Send(WebSocket ws, IEvent @event);
        Task Send(WebSocket ws, byte[] data);
    }

    public class EventSender : IEventSender
    {
        public async Task Send(WebSocket ws, IEvent @event)
        {
            var data = JsonSerializer.SerializeToUtf8Bytes(@event);
            await Send(ws, data);
        }
        public async Task Send(WebSocket ws, byte[] data)
        {
            var arraySegment = new ArraySegment<byte>(data, 0, data.Length);
            await ws.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
