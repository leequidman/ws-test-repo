using Common.Models.Requests.Abstract;

namespace Common.Models.Requests.UpdateResources;

public record InitUpdateResourceEventData(Guid? PlayerId, ResourceType? ResourceType, int? Amount) : IEventData;