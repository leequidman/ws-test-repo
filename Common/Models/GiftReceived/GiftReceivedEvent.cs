namespace Common.Models.GiftReceived;

public class GiftReceivedEvent : IEvent
{
    public EventType EventType => EventType.GiftReceived;
    public object? EventData { get; init; }

    public GiftReceivedEvent(GiftReceivedEventData eventData)
    {
        EventData = eventData;
    }
}