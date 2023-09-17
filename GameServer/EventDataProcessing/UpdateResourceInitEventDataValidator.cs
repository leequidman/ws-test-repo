using Common.Models.Requests.UpdateResources;
using FluentValidation;

namespace GameServer.EventDataProcessing;

public class UpdateResourceInitEventDataValidator : AbstractValidator<object?>
{
    public UpdateResourceInitEventDataValidator()
    {
        RuleFor(eventData => eventData)
            .NotNull()
            .Must(eventData => eventData is UpdateResourceInitEventData);

        When(eventData => eventData is UpdateResourceInitEventData,
            () =>
            {
                RuleFor(eventData => ((UpdateResourceInitEventData)eventData!).PlayerId)
                    .NotEmpty()
                    .NotEqual(Guid.Empty);

                RuleFor(eventData => ((UpdateResourceInitEventData)eventData!).ResourceType)
                    .NotNull()
                    .IsInEnum();

                RuleFor(eventData => ((UpdateResourceInitEventData)eventData!).Amount)
                    .NotNull()
                    .GreaterThan(0);
            });
    }
}