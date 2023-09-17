using Common.Models.Requests.Abstract;

namespace Common.Models.Requests.GiftReceived;

public class GiftReceivedEvent : IEvent
{
    public EventType EventType => EventType.GiftReceived;
    public object? EventData { get; init; }

    public GiftReceivedEvent(GiftReceivedEventData eventData)
    {
        EventData = eventData;
    }
}