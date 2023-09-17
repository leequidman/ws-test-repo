namespace Common.Models.SendGift;

public class SendGiftInitEvent : IEvent
{
    public EventType EventType => EventType.SendGiftInit;
    public object? EventData { get; init; }

    public SendGiftInitEvent(SendGiftInitEventData eventData)
    {
        EventData = eventData;
    }
}