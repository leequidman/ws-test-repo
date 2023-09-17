using Common.Models.Requests.Abstract;

namespace Common.Models.Requests.UpdateResources;

public record UpdateResourceInitEventData(Guid? PlayerId, ResourceType? ResourceType, int? Amount) : IEventData;