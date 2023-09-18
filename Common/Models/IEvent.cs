namespace Common.Models;

public interface IEvent
{
    EventType EventType { get; }

    // could replace 'object' with IEventData but System.Text.Json doesn't support interfaces
    // and this workaround is easier than writing a custom converters
    public object? EventData { get; init; }
}