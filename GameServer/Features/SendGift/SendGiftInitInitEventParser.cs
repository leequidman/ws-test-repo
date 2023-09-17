using System.Text.Json.Nodes;
using Common.EventHandling;
using Common.Models;
using Common.Models.Requests.Abstract;
using Common.Models.Requests.SendGift;
using Common.Models.Requests.UpdateResources;
using JetBrains.Annotations;

namespace GameServer.Features.SendGift;

[UsedImplicitly]
public class SendGiftInitInitEventParser : IEventParser
{
    public EventType EventType => EventType.SendGiftInit;

    public IEventData Parse(string jsonString)
    {
        var jsonObj = JsonNode.Parse(jsonString)!.AsObject();

        var eventData = jsonObj[nameof(IEvent.EventData)];
        if (eventData == null)
            throw new ArgumentException(IEventParser.BuildErrorMessage(jsonString, "", EventType));

        var senderIdNode = eventData[nameof(SendGiftInitEventData.SenderId)];
        if (senderIdNode == null)
            throw new ArgumentException(IEventParser.BuildErrorMessage(jsonString,
                $"{nameof(SendGiftInitEventData.SenderId)} was null.",
                EventType));

        var senderIdString = senderIdNode.ToString();
        if (!Guid.TryParse(senderIdString, out var senderId))
            throw new ArgumentException(IEventParser.BuildErrorMessage(
                jsonString,
                $"{nameof(SendGiftInitEventData.SenderId)} must be of type Guid.",
                EventType));

        var receiverIdNode = eventData[nameof(SendGiftInitEventData.ReceiverId)];
        if (receiverIdNode == null)
            throw new ArgumentException(IEventParser.BuildErrorMessage(
                jsonString,
                $"{nameof(SendGiftInitEventData.ReceiverId)} was null.",
                EventType));
        var receiverIdString = receiverIdNode.ToString();
        if (!Guid.TryParse(receiverIdString, out var receiverId))
            throw new ArgumentException(IEventParser.BuildErrorMessage(
                jsonString,
                $"{nameof(SendGiftInitEventData.ReceiverId)} must be of type Guid.",
                EventType));

        var resourceType = eventData[nameof(SendGiftInitEventData.Resource)];
        if (resourceType == null)
            throw new ArgumentException(IEventParser.BuildErrorMessage(
                jsonString,
                $"{nameof(SendGiftInitEventData.Resource)} was null.",
                EventType));
        var resourceTypeString = resourceType.ToString();
        if (!Enum.TryParse<ResourceType>(resourceTypeString, out var resource))
            throw new ArgumentException(IEventParser.BuildErrorMessage(
                jsonString,
                $"{nameof(SendGiftInitEventData.Resource)} must be of type {nameof(ResourceType)}.",
                EventType));

        var amountNode = eventData[nameof(SendGiftInitEventData.Amount)];
        if (amountNode == null)
            throw new ArgumentException(IEventParser.BuildErrorMessage(
                jsonString,
                $"{nameof(SendGiftInitEventData.Amount)} was null.",
                EventType));
        var amountString = amountNode.ToString();
        if (!int.TryParse(amountString, out var amount))
            throw new ArgumentException(
                IEventParser.BuildErrorMessage(jsonString,
                    $"{nameof(SendGiftInitEventData.Amount)} must be of type int.",
                    EventType));

        return new SendGiftInitEventData(senderId, receiverId, resource, amount);
    }
}