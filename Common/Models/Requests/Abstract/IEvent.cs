namespace Common.Models.Requests.Abstract;

public interface IEvent
{
    EventType EventType { get; }
    public object? EventData { get; init; }
}