using Common.Models.Requests.Abstract;

namespace Common.Models.Requests.UpdateResources;

public record UpdateResourcesEventData(ResourceType ResourceType, int Amount) : IEventData;
