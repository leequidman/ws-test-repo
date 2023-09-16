namespace Common.Models.Response.Login;

public class LoginResponseResult
{
    public LoginResponseResult(Guid playerId)
    {
        PlayerId = playerId;
    }

    public Guid PlayerId { get; init; }
}