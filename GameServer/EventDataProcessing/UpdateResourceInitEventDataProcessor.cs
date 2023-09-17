using Common.Models.Requests.UpdateResources;
using FluentValidation;

namespace GameServer.EventDataProcessing;

public class UpdateResourceInitEventDataProcessor
{
    private readonly UpdateResourceInitEventDataValidator _validator = new();
    public UpdateResourceInitEventData PrepareEventData(object? eventData)
    {
        _validator.ValidateAndThrow(eventData);
        return (UpdateResourceInitEventData)eventData!;
    }
}