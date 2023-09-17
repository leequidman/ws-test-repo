using System.Text.Json.Nodes;
using Common.EventHandling;
using Common.Models;
using Common.Models.Requests.Abstract;
using Common.Models.Requests.UpdateResources;

namespace GameServer.Features.UpdateResource;

public class UpdateResourceInitEventParser : IEventParser
{
    public EventType EventType => EventType.UpdateResourceInit;
    public IEventData Parse(string? jsonString)
    {
        var jsonObj = JsonNode.Parse(jsonString!)!.AsObject();

        var eventData = jsonObj[nameof(IEvent.EventData)];
        if (eventData == null)
            throw new ArgumentException($"Invalid data format for EventType '{EventType}'." +
                                        $"{Environment.NewLine}{jsonString}: {jsonString}");

        var playerIdNode = eventData[nameof(UpdateResourceInitEventData.PlayerId)];
        if (playerIdNode == null)
            throw new ArgumentException(
                $"Invalid data format for EventType '{EventType}'. " +
                $"{nameof(UpdateResourceInitEventData.PlayerId)} was null." +
                $"{Environment.NewLine}{jsonString}: {jsonString}");

        var playerIdString = playerIdNode.ToString();
        if (!Guid.TryParse(playerIdString, out var playerId))
            throw new ArgumentException(
                $"Invalid data format for EventType '{EventType}'. " +
                $"{nameof(UpdateResourceInitEventData.PlayerId)} must be of type Guid." +
                $"{Environment.NewLine}{jsonString}: {jsonString}");

        var resourceTypeNode = eventData[nameof(UpdateResourceInitEventData.ResourceType)];
        if (resourceTypeNode == null)
            throw new ArgumentException(
                $"Invalid data format for EventType '{EventType}'. " +
                $"{nameof(UpdateResourceInitEventData.ResourceType)} was null." +
                $"{Environment.NewLine}{jsonString}: {jsonString}");

        var resourceTypeString = resourceTypeNode.ToString();
        if (!Enum.TryParse<ResourceType>(resourceTypeString, out var resourceType))
            throw new ArgumentException(
                $"Invalid data format for EventType '{EventType}'. " +
                $"{nameof(UpdateResourceInitEventData.ResourceType)} must be of type {nameof(ResourceType)}." +
                $"{Environment.NewLine}{jsonString}: {jsonString}");

        var amountNode = eventData[nameof(UpdateResourceInitEventData.Amount)];
        if (amountNode == null)
            throw new ArgumentException(
                $"Invalid data format for EventType '{EventType}'. " +
                $"{nameof(UpdateResourceInitEventData.Amount)} was null." +
                $"{Environment.NewLine}{jsonString}: {jsonString}");

        var amountString = amountNode.ToString();
        if (!int.TryParse(amountString, out var amount))
            throw new ArgumentException(
                $"Invalid data format for EventType '{EventType}'. " +
                $"{nameof(UpdateResourceInitEventData.Amount)} must be of type int." +
                $"{Environment.NewLine}{jsonString}: {jsonString}");

        return new UpdateResourceInitEventData(playerId, resourceType, amount);
    }
}