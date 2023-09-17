using Common.Models.Requests.Abstract;

namespace Common.Models.Requests.UpdateResources;

public record InitUpdateResourceEvent : IEvent
{
    public EventType EventType => EventType.UpdateResourceInit;
    public object? EventData { get; init; }

    public InitUpdateResourceEvent(InitUpdateResourceEventData eventData)
    {
        EventData = eventData;
    }
}