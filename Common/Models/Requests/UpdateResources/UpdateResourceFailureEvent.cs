using Common.Models.Requests.Abstract;

namespace Common.Models.Requests.UpdateResources;

public record UpdateResourceFailureEvent : IEvent
{
    public EventType EventType => EventType.UpdateResourceFailure;
    public object? EventData { get; init; }

    public UpdateResourceFailureEvent(UpdateResourceFailureEventData eventData)
    {
        EventData = eventData;
    }
}