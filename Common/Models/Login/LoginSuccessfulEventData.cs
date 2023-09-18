namespace Common.Models.Login;

public class LoginSuccessfulEventData
{
    public LoginSuccessfulEventData(Guid playerId)
    {
        PlayerId = playerId;
    }

    public Guid PlayerId { get; init; }
}