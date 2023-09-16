using Common.Models.Requests.Abstract;

namespace Common.Models.Requests.UpdateResources;

public record UpdateResourceEvent : IEvent
{
    public EventType EventType => EventType.UpdateResources;
    public object? EventData { get; init; }

    public UpdateResourceEvent(UpdateResourcesEventData eventData)
    {
        EventData = eventData;
    }

    public UpdateResourceEvent(object? eventData)
    {
        EventData = eventData;
    }
}