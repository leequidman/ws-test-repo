using Common.Models.Requests.Abstract;

namespace Common.Models.Requests.UpdateResources;

public record UpdateResourceSuccessEvent: IEvent
{
    public EventType EventType => EventType.UpdateResourceSuccess;
    public object? EventData { get; init; }

    public UpdateResourceSuccessEvent(UpdateResourceSuccessEventData eventData)
    {
        EventData = eventData;
    }
}