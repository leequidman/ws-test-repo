using Common.Models.Requests.Abstract;

namespace Common.Models.Requests.UpdateResources;

public record UpdateResourceSuccessEventData(
    Guid? PlayerId,
    ResourceType? ResourceType,
    int? NewAmount) : IEventData;