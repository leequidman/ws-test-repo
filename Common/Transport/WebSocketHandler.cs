using System.Net.WebSockets;
using System.Text.Json;
using Common.EventHandling;
using Common.Models.Requests.Abstract;

namespace Common.Transport;

public class WebSocketHandler : IWebSocketHandler
{
    private readonly IBaseMessageHandler _baseMessageHandler;

    public WebSocketHandler(IBaseMessageHandler baseMessageHandler)
    {
        _baseMessageHandler = baseMessageHandler;
    }

    public async Task ReceiveMessage(WebSocket ws)
    {
        var buffer = new byte[1024 * 4];
        while (ws.State == WebSocketState.Open)
        {
            var result = await ws.ReceiveAsync(new(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                await _baseMessageHandler.Handle(result, ws, buffer);
            }
            else if (result.MessageType == WebSocketMessageType.Close || ws.State == WebSocketState.Aborted)
            {
                await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
        }
    }

    public async Task SendEvent(WebSocket ws, IEvent @event)
    {
        var data = JsonSerializer.SerializeToUtf8Bytes(@event);
        await Send(ws, data);
    }
    private async Task Send(WebSocket ws, byte[] data)
    {
        var arraySegment = new ArraySegment<byte>(data, 0, data.Length);
        await ws.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
    }

}