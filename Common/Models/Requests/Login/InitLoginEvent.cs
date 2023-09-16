using Common.Models.Requests.Abstract;

namespace Common.Models.Requests.Login;

public class InitLoginEvent : IEvent
{
    public EventType EventType => EventType.InitLogin;
    public object? EventData { get; init; }

    public InitLoginEvent(InitLoginEventData eventData)
    {
        EventData = eventData;
    }
}