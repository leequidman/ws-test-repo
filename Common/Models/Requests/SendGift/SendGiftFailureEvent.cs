using Common.Models.Requests.Abstract;

namespace Common.Models.Requests.SendGift;

public class SendGiftFailureEvent : IEvent
{
    public EventType EventType => EventType.SendGiftFailure;
    public object? EventData { get; init; }

    public SendGiftFailureEvent(SendGiftFailureEventData eventData)
    {
        EventData = eventData;
    }
}