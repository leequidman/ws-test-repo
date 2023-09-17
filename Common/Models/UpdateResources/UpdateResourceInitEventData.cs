namespace Common.Models.UpdateResources;

public record UpdateResourceInitEventData(Guid? PlayerId, ResourceType? ResourceType, int? Amount) : IEventData;