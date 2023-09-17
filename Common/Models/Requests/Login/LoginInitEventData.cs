using Common.Models.Requests.Abstract;

namespace Common.Models.Requests.Login;

public record LoginInitEventData(Guid DeviceId) : IEventData;