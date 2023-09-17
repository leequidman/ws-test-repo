using System.Net.WebSockets;
using Common.EventHandling;
using Common.Models;
using Common.Models.Requests.Abstract;
using Common.Models.Requests.Login;
using Common.Models.Requests.UpdateResources;
using Common.Transport;
using GameServer.EventDataProcessing;
using GameServer.Services;
using JetBrains.Annotations;
using ILogger = Serilog.ILogger;

namespace GameServer.Handlers;

[UsedImplicitly]
public class UpdateResourcesHandler : IEventHandler
{
    private readonly IPlayersService _playersService;
    private readonly ILogger _logger;
    private readonly IWebSocketHandler _webSocketHandler;
    private readonly UpdateResourceInitEventDataProcessor _dataProcessor = new();

    public UpdateResourcesHandler(IPlayersService playersService, ILogger logger, IWebSocketHandler webSocketHandler)
    {
        _playersService = playersService;
        _logger = logger;
        _webSocketHandler = webSocketHandler;
    }

    public EventType EventType => EventType.UpdateResourceInit;

    public async Task Handle(object? eventData, WebSocket ws)
    {
        var data = _dataProcessor.PrepareEventData(eventData);

        IEvent updateResourceEvent;

        try
        {
            await _playersService.UpdateResources((Guid)data.PlayerId, data.ResourceType.Value, data.Amount.Value);
            updateResourceEvent = new UpdateResourceSuccessEvent(new(data.PlayerId.Value, data.ResourceType.Value, data.Amount.Value));  
        }
        catch (Exception e)
        {
            _logger.Error(e, $"Failed to update resources for player {data.PlayerId}.");
            updateResourceEvent = new UpdateResourceFailureEvent(
                new($"Failed to update resources for player {data.PlayerId}: {e.Message}"));       
        }
        await _webSocketHandler.SendEvent(ws, updateResourceEvent);

    }
}