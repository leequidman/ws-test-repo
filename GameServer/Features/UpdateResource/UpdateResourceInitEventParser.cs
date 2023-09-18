using System.Text.Json.Nodes;
using Common.Models;
using Common.Models.UpdateResources;
using GameServer.Transport;
using JetBrains.Annotations;

namespace GameServer.Features.UpdateResource;

[UsedImplicitly]
public class UpdateResourceInitEventParser : IEventParser
{
    public EventType EventType => EventType.UpdateResourceInit;

    public IEventData Parse(string jsonString)
    {
        var jsonObj = JsonNode.Parse(jsonString)!.AsObject();


        var eventData = jsonObj[nameof(IEvent.EventData)];
        if (eventData == null)
            throw new ArgumentException(IEventParser.BuildErrorMessage(jsonString, "", EventType));

        var playerIdNode = eventData[nameof(UpdateResourceInitEventData.PlayerId)];
        if (playerIdNode == null)
            throw new ArgumentException(IEventParser.BuildErrorMessage(
                jsonString,
                $"{nameof(UpdateResourceInitEventData.PlayerId)} was null.",
                EventType));

        var playerIdString = playerIdNode.ToString();
        if (!Guid.TryParse(playerIdString, out var playerId))
            throw new ArgumentException(IEventParser.BuildErrorMessage(
                jsonString,
                $"{nameof(UpdateResourceInitEventData.PlayerId)} must be of type Guid.",
                EventType));

        var resourceTypeNode = eventData[nameof(UpdateResourceInitEventData.ResourceType)];
        if (resourceTypeNode == null)
            throw new ArgumentException(IEventParser.BuildErrorMessage(
                jsonString,
                $"{nameof(UpdateResourceInitEventData.ResourceType)} was null.",
                EventType));

        var resourceTypeString = resourceTypeNode.ToString();
        if (!Enum.TryParse<ResourceType>(resourceTypeString, out var resourceType))
            throw new ArgumentException(IEventParser.BuildErrorMessage(
                jsonString,
                $"{nameof(UpdateResourceInitEventData.ResourceType)} must be of type {nameof(ResourceType)}.",
                EventType));

        var amountNode = eventData[nameof(UpdateResourceInitEventData.Amount)];
        if (amountNode == null)
            throw new ArgumentException(IEventParser.BuildErrorMessage(
                jsonString,
                $"{nameof(UpdateResourceInitEventData.Amount)} was null.",
                EventType));

        var amountString = amountNode.ToString();
        if (!int.TryParse(amountString, out var amount))
            throw new ArgumentException(IEventParser.BuildErrorMessage(
                jsonString,
                $"{nameof(UpdateResourceInitEventData.Amount)} must be of type int.",
                EventType));

        return new UpdateResourceInitEventData(playerId, resourceType, amount);
    }
}