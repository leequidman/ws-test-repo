using Common.Models.Requests.Login;
using FluentValidation;

namespace GameServer.EventDataProcessing;

public class LoginInitEventDataValidator : AbstractValidator<object?>
{
    public LoginInitEventDataValidator()
    {
        RuleFor(eventData => eventData)
            .NotNull()
            .Must(eventData => eventData is LoginInitEventData);

        When(eventData => eventData is LoginInitEventData,
            () =>
            {
                RuleFor(eventData => ((LoginInitEventData)eventData!).DeviceId)
                    .NotEmpty()
                    .NotEqual(Guid.Empty);
            });
    }
}