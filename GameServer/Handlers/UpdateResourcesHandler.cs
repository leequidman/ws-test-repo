using System.Net.WebSockets;
using Common.EventHandling;
using Common.Models;
using Common.Models.Requests.Abstract;
using Common.Models.Requests.UpdateResources;
using Common.Transport;
using GameServer.Services;
using ILogger = Serilog.ILogger;

namespace GameServer.Handlers;

public class UpdateResourcesHandler : IEventHandler
{
    private readonly IPlayersService _playersService;
    private readonly ILogger _logger;
    private readonly IWebSocketHandler _webSocketHandler;

    public UpdateResourcesHandler(IPlayersService playersService, ILogger logger, IWebSocketHandler webSocketHandler)
    {
        _playersService = playersService;
        _logger = logger;
        _webSocketHandler = webSocketHandler;
    }

    public EventType EventType => EventType.InitUpdateResource;
    public async Task Handle(object? eventData, WebSocket ws)
    {
        if (eventData is null)
            throw new ArgumentException($"Expected {nameof(eventData)} to be non-null");

        if (eventData is not InitUpdateResourceEventData data)
            throw new ArgumentException($"Expected {nameof(InitUpdateResourceEventData)} but got {eventData.GetType().Name}");

        if (data == null)
            throw new ArgumentException($"Expected {nameof(data)} to be non-null");

        if (data.PlayerId is null || data.PlayerId == Guid.Empty)    
            throw new ArgumentException($"Expected {nameof(data.PlayerId)} to be non-empty");       

        if (data.ResourceType is null)
            throw new ArgumentException($"Expected {nameof(data.ResourceType)} to be non-null");

        if (data.Amount is null)
            throw new ArgumentException($"Expected {nameof(data.Amount)} to be non-null");

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