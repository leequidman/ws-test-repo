using System.Net.WebSockets;
using System.Text.Json;
using Common.EventHandling;
using Common.Models.Requests.Abstract;
using GameServer.Services;

namespace GameServer.Transport;

public class WebSocketHandler : IWebSocketHandler
{
    private readonly IBaseMessageHandler _baseMessageHandler;
    private readonly Serilog.ILogger _logger;
    private readonly IConnectionService _connectionService;


    public WebSocketHandler(IBaseMessageHandler baseMessageHandler, Serilog.ILogger logger, IConnectionService connectionService)
    {
        _baseMessageHandler = baseMessageHandler;
        _logger = logger;
        _connectionService = connectionService;
    }

    public async Task ReceiveMessage(WebSocket ws)
    {
        var buffer = new byte[Common.Constants.WebSocketReceiveBufferSize];
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
                    _connectionService.SetOffline(ws);
                    await ws.CloseAsync(result.CloseStatus!.Value, result.CloseStatusDescription, CancellationToken.None);
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
            _logger.Information($"Sending event: {JsonSerializer.Serialize(@event)}");
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