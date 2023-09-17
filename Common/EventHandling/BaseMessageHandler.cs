using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Nodes;
using Common.Models;
using Common.Models.Requests.Abstract;

namespace Common.EventHandling;

public interface IBaseMessageHandler
{
    Task Handle(WebSocketReceiveResult webSocketReceiveResult, WebSocket ws, byte[] buffer);
}

public class BaseMessageHandler : IBaseMessageHandler
{
    private readonly IEventHandlerProvider _eventHandlerProvider;
    private readonly IEventParserProvider _eventParserProvider;

    public BaseMessageHandler(IEventHandlerProvider eventHandlerProvider, IEventParserProvider eventParserProvider)
    {
        _eventHandlerProvider = eventHandlerProvider;
        _eventParserProvider = eventParserProvider;
    }

    public async Task Handle(WebSocketReceiveResult webSocketReceiveResult, WebSocket ws, byte[] buffer)
    {
        var message = Encoding.UTF8.GetString(buffer, 0, webSocketReceiveResult.Count);

        var eventType = GetEventType(message);

        var parser = _eventParserProvider.GetParser(eventType);

        var eventData = parser.Parse(message);

        var eventHandlers = _eventHandlerProvider.GetHandlers(eventType);

        // (michael_v): point of extension in case multiple handlers are needed for one event
        foreach (var eventHandler in eventHandlers)
            await eventHandler.Handle(eventData, ws);
    }

    private static EventType GetEventType(string jsonString)
    {
        var jsonObj = JsonNode.Parse(jsonString)?.AsObject();
        if (jsonObj == null)
            throw new ArgumentException($"Invalid json: {jsonString}");

        var jsonNode = jsonObj[nameof(IEvent.EventType)];
        if (jsonNode == null)
            throw new ArgumentException($"Invalid json: {jsonString}. " +
                                        $"{Environment.NewLine}Failed to get EventType.");

        var evenTypeString = jsonNode.ToString();
        if (!Enum.TryParse<EventType>(evenTypeString, out var type))
            throw new ArgumentException($"Unknown event type: {evenTypeString}");

        return type;
    }
}