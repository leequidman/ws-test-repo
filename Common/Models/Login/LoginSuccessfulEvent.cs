namespace Common.Models.Login;

public class LoginSuccessfulEvent : IEvent
{
    public EventType EventType => EventType.LoginSuccessful;
    public object? EventData { get; init; }

    public LoginSuccessfulEvent(LoginSuccessfulEventData eventData)
    {
        EventData = eventData;
    }
}