namespace Common.Models.UpdateResources;

public record UpdateResourceSuccessEventData(
    Guid? PlayerId,
    ResourceType? ResourceType,
    int? NewAmount) : IEventData;