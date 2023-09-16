using Common.Models.Response.Abstract;

namespace Common.Models.Response.Login;

public class LoginResponse : IResponse
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public object? ResponseResult { get; init; }

    public LoginResponse(LoginResponseResult responseResult)
    {
        ResponseResult = responseResult;
    }
}