using Common.Models.Requests.Abstract;
using Common.Models.Requests.Login;
using Serilog;

namespace Common.Models;

public class LoginRequestHandler : IRequestHandler
{
    public EventType EventType => EventType.InitLogin;

    public LoginRequestHandler(ILogger logger)
    {
    }

    public async Task<IEvent> Handle(IEventData eventData)
    {
        var loginRequestData = eventData as InitLoginEventData;

        var loginResponseResult = new LoginSuccessfulEventData(Guid.NewGuid());

        var loginResponse = new LoginSuccessfulEvent(loginResponseResult);

        return loginResponse;
    }
}