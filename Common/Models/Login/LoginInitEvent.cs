namespace Common.Models.Login;

public class LoginInitEvent : IEvent
{
    public EventType EventType => EventType.LoginInit;
    public object? EventData { get; init; }

    public LoginInitEvent(LoginInitEventData eventData)
    {
        EventData = eventData;
    }
}