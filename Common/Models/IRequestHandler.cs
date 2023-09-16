using Common.Models.Requests.Abstract;
using Common.Models.Response.Abstract;

namespace Common.Models;

public interface IRequestHandler
{
    RequestType RequestType { get; }

    Task<IResponse> Handle(IRequestData requestData);
}