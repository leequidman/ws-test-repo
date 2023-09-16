using System.Net.WebSockets;
using System.Text;
using Common.Models.Requests.Abstract;
using Common.Models.Requests.Login;
using Common.Models.Requests.UpdateResources;
using Common.Models;
using System.Text.Json.Nodes;

namespace GameServer.Handlers
{
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
                case EventType.InitLogin:
                    var node = jsonObj[nameof(IEvent.EventData)]?[nameof(InitLoginEventData.DeviceId)];
                    if (node == null)
                        throw new ArgumentException($"Invalid data format for EventType '{evenTypeString}'." +
                                                    $"{Environment.NewLine}{jsonString}: {jsonString}");
                    return new InitLoginEvent(new(Guid.Parse(node.ToString())));

                case EventType.UpdateResources:
                    var eventData = jsonObj[nameof(IEvent.EventData)];
                    if (eventData == null)
                        throw new ArgumentException($"Invalid data format for EventType '{evenTypeString}'." +
                                                    $"{Environment.NewLine}{jsonString}: {jsonString}");
                    var resourceTypeNode = eventData[nameof(UpdateResourcesEventData.ResourceType)];
                    if (resourceTypeNode == null)
                        throw new ArgumentException(
                            $"Invalid data format for EventType '{evenTypeString}'. " +
                            $"{nameof(UpdateResourcesEventData.ResourceType)} was null." +
                            $"{Environment.NewLine}{jsonString}: {jsonString}");
                    
                    var resourceTypeString = resourceTypeNode.ToString();
                    if (!Enum.TryParse<ResourceType>(resourceTypeString, out var resourceType))
                        throw new ArgumentException(
                            $"Invalid data format for EventType '{evenTypeString}'. " +
                            $"{nameof(UpdateResourcesEventData.ResourceType)} must be of type {nameof(ResourceType)}." +
                            $"{Environment.NewLine}{jsonString}: {jsonString}");  

                    var amountNode = eventData[nameof(UpdateResourcesEventData.Amount)];
                    if (amountNode == null)
                        throw new ArgumentException(
                            $"Invalid data format for EventType '{evenTypeString}'. " +
                            $"{nameof(UpdateResourcesEventData.Amount)} was null." +
                            $"{Environment.NewLine}{jsonString}: {jsonString}");

                    var amountString = amountNode.ToString();
                    if (!int.TryParse(amountString, out var amount))
                        throw new ArgumentException(
                            $"Invalid data format for EventType '{evenTypeString}'. " +
                            $"{nameof(UpdateResourcesEventData.Amount)} must be of type int." +
                            $"{Environment.NewLine}{jsonString}: {jsonString}");

                    return new UpdateResourceEvent(new(resourceType, amount));

                // case EventType.SendGift:
                //     break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}