using Common.Models.Requests.Abstract;

namespace Common.Models.Requests.Login;

public record InitLoginEventData(Guid DeviceId) : IEventData;