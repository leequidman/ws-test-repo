namespace Common.Models.Login;

public record LoginInitEventData(Guid DeviceId) : IEventData;