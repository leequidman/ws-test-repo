using Common.Models.Requests.Abstract;

namespace Common.Models.Requests.UpdateResources;

public record UpdateResourceInitEvent : IEvent
{
    public EventType EventType => EventType.UpdateResourceInit;
    public object? EventData { get; init; }

    public UpdateResourceInitEvent(UpdateResourceInitEventData eventData)
    {
        EventData = eventData;
    }
}