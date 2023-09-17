using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Nodes;
using Common.Models;
using Common.Models.Requests.Abstract;
using Common.Models.Requests.Login;
using Common.Models.Requests.UpdateResources;

namespace Common.EventHandling
{
    public interface IBaseMessageHandler
    {
        Task Handle(WebSocketReceiveResult webSocketReceiveResult, WebSocket ws, byte[] buffer);
    }

    public class BaseMessageHandler : IBaseMessageHandler
    {
        private readonly IEventHandlerProvider _eventHandlerProvider;

        public BaseMessageHandler(IEventHandlerProvider eventHandlerProvider)
        {
            _eventHandlerProvider = eventHandlerProvider;
        }

        public async Task Handle(WebSocketReceiveResult webSocketReceiveResult, WebSocket ws, byte[] buffer)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, webSocketReceiveResult.Count);
            var @event = Parse(message);

            var eventHandlers = _eventHandlerProvider.GetHandlers(@event.EventType);

            // (michael_v): point of extension in case multiple handlers are needed for one event
            foreach (var eventHandler in eventHandlers)
                await eventHandler.Handle(@event.EventData, ws);
        }

        private static IEvent Parse(string jsonString)
        {
            // todo: to FluentAssertions
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

            switch (type)
            {
                case EventType.LoginInit:
                    var node = jsonObj[nameof(IEvent.EventData)]?[nameof(LoginInitEventData.DeviceId)];
                    if (node == null)
                        throw new ArgumentException($"Invalid data format for EventType '{evenTypeString}'." +
                                                    $"{Environment.NewLine}{jsonString}: {jsonString}");
                    return new LoginInitEvent(new(Guid.Parse(node.ToString())));

                case EventType.UpdateResourceInit:
                    var eventData = jsonObj[nameof(IEvent.EventData)];
                    if (eventData == null)
                        throw new ArgumentException($"Invalid data format for EventType '{evenTypeString}'." +
                                                    $"{Environment.NewLine}{jsonString}: {jsonString}");

                    var playerIdNode = eventData[nameof(InitUpdateResourceEventData.PlayerId)];
                    if (playerIdNode == null)
                        throw new ArgumentException(
                            $"Invalid data format for EventType '{evenTypeString}'. " +
                            $"{nameof(InitUpdateResourceEventData.PlayerId)} was null." +
                            $"{Environment.NewLine}{jsonString}: {jsonString}");

                    var playerIdString = playerIdNode.ToString();
                    if (!Guid.TryParse(playerIdString, out var playerId))
                        throw new ArgumentException(
                            $"Invalid data format for EventType '{evenTypeString}'. " +
                            $"{nameof(InitUpdateResourceEventData.PlayerId)} must be of type Guid." +
                            $"{Environment.NewLine}{jsonString}: {jsonString}");

                    var resourceTypeNode = eventData[nameof(InitUpdateResourceEventData.ResourceType)];
                    if (resourceTypeNode == null)
                        throw new ArgumentException(
                            $"Invalid data format for EventType '{evenTypeString}'. " +
                            $"{nameof(InitUpdateResourceEventData.ResourceType)} was null." +
                            $"{Environment.NewLine}{jsonString}: {jsonString}");

                    var resourceTypeString = resourceTypeNode.ToString();
                    if (!Enum.TryParse<ResourceType>(resourceTypeString, out var resourceType))
                        throw new ArgumentException(
                            $"Invalid data format for EventType '{evenTypeString}'. " +
                            $"{nameof(InitUpdateResourceEventData.ResourceType)} must be of type {nameof(ResourceType)}." +
                            $"{Environment.NewLine}{jsonString}: {jsonString}");

                    var amountNode = eventData[nameof(InitUpdateResourceEventData.Amount)];
                    if (amountNode == null)
                        throw new ArgumentException(
                            $"Invalid data format for EventType '{evenTypeString}'. " +
                            $"{nameof(InitUpdateResourceEventData.Amount)} was null." +
                            $"{Environment.NewLine}{jsonString}: {jsonString}");

                    var amountString = amountNode.ToString();
                    if (!int.TryParse(amountString, out var amount))
                        throw new ArgumentException(
                            $"Invalid data format for EventType '{evenTypeString}'. " +
                            $"{nameof(InitUpdateResourceEventData.Amount)} must be of type int." +
                            $"{Environment.NewLine}{jsonString}: {jsonString}");

                    return new InitUpdateResourceEvent(new(playerId, resourceType, amount));

                // case EventType.SendGiftInit:
                //     break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}