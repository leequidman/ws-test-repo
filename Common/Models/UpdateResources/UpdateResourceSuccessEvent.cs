namespace Common.Models.UpdateResources;

public record UpdateResourceSuccessEvent : IEvent
{
    public EventType EventType => EventType.UpdateResourceSuccess;
    public object? EventData { get; init; }

    public UpdateResourceSuccessEvent(UpdateResourceSuccessEventData eventData)
    {
        EventData = eventData;
    }
}