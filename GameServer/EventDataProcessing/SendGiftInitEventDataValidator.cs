using Common.Models.Requests.SendGift;
using FluentValidation;

namespace GameServer.EventDataProcessing;

public class SendGiftInitEventDataValidator : AbstractValidator<object?>
{
    public SendGiftInitEventDataValidator()
    {
        RuleFor(eventData => eventData)
            .NotNull()
            .Must(eventData => eventData is SendGiftInitEventData);

        When(eventData => eventData is SendGiftInitEventData,
            () =>
            {
                RuleFor(eventData => ((SendGiftInitEventData)eventData!).SenderId)
                    .NotEmpty()
                    .NotEqual(Guid.Empty);

                RuleFor(eventData => ((SendGiftInitEventData)eventData!).ReceiverId)
                    .NotEmpty()
                    .NotEqual(Guid.Empty);

                RuleFor(eventData => ((SendGiftInitEventData)eventData!).Resource)
                    .NotNull()
                    .IsInEnum();

                RuleFor(eventData => ((SendGiftInitEventData)eventData!).Amount)
                    .NotNull()
                    .GreaterThan(0);
            });
    }
}