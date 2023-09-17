using Common.Models.Requests.Abstract;

namespace Common.Models.Requests.SendGift;

public class SendGiftSuccessEvent : IEvent
{
    public EventType EventType => EventType.SendGiftSuccess;
    public object? EventData { get; init; }

    public SendGiftSuccessEvent(SendGiftSuccessEventData eventData)
    {
        EventData = eventData;
    }
}