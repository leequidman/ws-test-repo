namespace Common.Models.Requests.Abstract;

public interface IRequest
{
    RequestType RequestType { get; }
    IRequestData RequestData { get; }
}