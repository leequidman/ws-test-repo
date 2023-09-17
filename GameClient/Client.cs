using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Common.Models;
using Common.Models.Login;
using Common.Models.SendGift;
using Common.Models.UpdateResources;
using Serilog;

namespace GameClient;

public class Client
{
    private readonly Guid _deviceId;
    private readonly ClientWebSocket _ws;
    private readonly ILogger _logger = Log.ForContext<Client>();
    public readonly ConcurrentQueue<string> Messages = new();


    public Client(Guid deviceId)
    {
        _deviceId = deviceId;
        _ws = new();
    }

    public async Task Login()
    {
        var request = new LoginInitEvent(new(_deviceId));
        var bytes = JsonSerializer.SerializeToUtf8Bytes(request);
        await _ws.SendAsync(new(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async Task SendGift(Guid senderId, Guid receiverId, ResourceType resourceType, int amount)
    {
        var request = new SendGiftInitEvent(new(senderId, receiverId, resourceType, amount));
        var bytes = JsonSerializer.SerializeToUtf8Bytes(request);
        await _ws.SendAsync(new(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async Task UpdateResource(ResourceType resourceType, int amount)
    {
        var request = new UpdateResourceInitEvent(new(_deviceId, resourceType, amount));
        var bytes = JsonSerializer.SerializeToUtf8Bytes(request);
        await _ws.SendAsync(new(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async Task<Task> Connect()
    {
        _logger.Information($"Start connecting with deviceId '{_deviceId}'");

        try
        {
            await _ws.ConnectAsync(new("ws://" + Common.Constants.EndpointUrl + "/ws"), CancellationToken.None);
            _logger.Information($"Connected with deviceId '{_deviceId}'");

            return ReceiveMessages();
        }
        catch (Exception exception)
        {
            _logger.Error(exception, $"Error while connecting with deviceId '{_deviceId}'");
            throw;
        }
    }

    public async Task Disconnect()
    {
        await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
    }

    private async Task ReceiveMessages()
    {
        var buffer = new byte[Common.Constants.WebSocketReceiveBufferSize];
        while (_ws.State == WebSocketState.Open)
        {
            try
            {
                var result = await _ws.ReceiveAsync(new(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _logger.Information($"Connection closed with deviceId '{_deviceId}'");
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Messages.Enqueue(message);
                _logger.Information($"Received message: {message}");
            }
            catch (Exception exception)
            {
                _logger.Error(exception, $"Error while receiving with deviceId '{_deviceId}'");
            }
        }
    }
}