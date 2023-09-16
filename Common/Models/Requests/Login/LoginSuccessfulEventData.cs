namespace Common.Models.Requests.Login;

public class LoginSuccessfulEventData
{
    public LoginSuccessfulEventData(Guid playerId)
    {
        PlayerId = playerId;
    }

    public Guid PlayerId { get; init; }
}

public class LoginFailedEventData
{
    public LoginFailedEventData(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; init; }
}