using Common.Models.Requests.SendGift;
using FluentValidation;

namespace GameServer.EventDataProcessing;

public class SendGiftInitEventDataProcessor
{
    private readonly SendGiftInitEventDataValidator _validator = new();

    public SendGiftInitEventData PrepareEventData(object? eventData)
    {
        _validator.ValidateAndThrow(eventData);
        return (SendGiftInitEventData)eventData!;
    }
}