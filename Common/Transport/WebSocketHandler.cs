using System.Net.WebSockets;
using System.Text.Json;
using Common.EventHandling;
using Common.Models.Requests.Abstract;
using Serilog;

namespace Common.Transport;

public class WebSocketHandler : IWebSocketHandler
{
    private readonly IBaseMessageHandler _baseMessageHandler;
    private readonly Serilog.ILogger _logger;

    public WebSocketHandler(IBaseMessageHandler baseMessageHandler, ILogger logger)
    {
        _baseMessageHandler = baseMessageHandler;
        _logger = logger;
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
        if (ws.State != WebSocketState.Open)
        {
            _logger.Warning($"Can't send message to {ws.CloseStatus} socket.");
            return;

        }
        var arraySegment = new ArraySegment<byte>(data, 0, data.Length);
        await ws.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
    }

}