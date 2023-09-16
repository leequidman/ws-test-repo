using Common.Models.Requests.Abstract;

namespace Common.Models.Requests.UpdateResources;

public record InitUpdateResourceEvent : IEvent
{
    public EventType EventType => EventType.InitUpdateResource;
    public object? EventData { get; init; }

    public InitUpdateResourceEvent(InitUpdateResourceEventData eventData)
    {
        EventData = eventData;
    }
}