namespace Common.Models.Requests.Login;

public class LoginRequest
{
    public RequestType RequestType => RequestType.Login;
    public LoginRequestData RequestData { get; }

    public LoginRequest(LoginRequestData requestData)
    {
        RequestData = requestData;
    }
}