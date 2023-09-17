using Common.Models.Requests.Abstract;

namespace Common.Models.Requests.SendGift;

public class SendGiftInitEvent : IEvent
{
    public EventType EventType => EventType.SendGiftInit;
    public object? EventData { get; init; }

    public SendGiftInitEvent(SendGiftInitEventData eventData)
    {
        EventData = eventData;
    }
}