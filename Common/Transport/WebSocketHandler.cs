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
            try
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
            catch (Exception e)
            {
                _logger.Information(e,"Failed to receive message." +
                                    "WS info: " +
                                    $"State: {ws.State}, " +
                                    $"CloseStatus: {ws.CloseStatus}, " +
                                    $"CloseStatusDescription: {ws.CloseStatusDescription}");

            }
        }
    }

    public async Task SendEvent(WebSocket ws, IEvent @event)
    {
        try
        {
            var data = JsonSerializer.SerializeToUtf8Bytes(@event);
            await Send(ws, data);
        }
        catch (Exception e)
        {
            _logger.Error(e,
                $"Failed to send event {@event.EventType}." +
                "WS info: " +
                $"State: {ws.State}, " +
                $"CloseStatus: {ws.CloseStatus}, " +
                $"CloseStatusDescription: {ws.CloseStatusDescription}");
        }
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