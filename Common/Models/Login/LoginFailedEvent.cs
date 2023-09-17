namespace Common.Models.Login;

public class LoginFailedEvent : IEvent
{
    public EventType EventType => EventType.LoginFailed;
    public object? EventData { get; init; }

    public LoginFailedEvent(LoginFailedEventData eventData)
    {
        EventData = eventData;
    }
}