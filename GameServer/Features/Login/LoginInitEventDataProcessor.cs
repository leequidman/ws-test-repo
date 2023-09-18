using Common.Models.Login;
using FluentValidation;

namespace GameServer.Features.Login;

public class LoginInitEventDataProcessor
{
    private readonly LoginInitEventDataValidator _validator = new();
    public LoginInitEventData PrepareEventData(object? eventData)
    {
        _validator.ValidateAndThrow(eventData);
        return (LoginInitEventData)eventData!;
    }
}