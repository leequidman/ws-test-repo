using Common.Models.Requests.Abstract;
using Common.Models.Response.Abstract;
using Common.Models.Response.Login;
using Serilog;

namespace Common.Models;

public class LoginRequestHandler : IRequestHandler
{
    public RequestType RequestType => RequestType.Login;

    public LoginRequestHandler(ILogger logger)
    {
    }

    public async Task<IResponse> Handle(IRequestData requestData)
    {
        var loginRequestData = requestData as LoginRequestData;

        var loginResponseResult = new LoginResponseResult
        {
            PlayerId = Guid.NewGuid()
        };

        var loginResponse = new LoginResponse(loginResponseResult);

        return loginResponse;
    }
}