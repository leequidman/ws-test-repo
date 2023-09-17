using Common.Models.Requests.Login;
using FluentValidation;

namespace GameServer.EventDataProcessing;

public class LoginInitEventDataProcessor
{
    private readonly LoginInitEventDataValidator _validator = new();
    public LoginInitEventData PrepareEventData(object? eventData)
    {
        _validator.ValidateAndThrow(eventData);
        return (LoginInitEventData)eventData!;
    }
}