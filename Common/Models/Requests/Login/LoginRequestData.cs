using Common.Models.Requests.Abstract;

namespace Common.Models;

public class LoginRequestData : IRequestData
{
    public Guid DeviceId { get; set; }
}