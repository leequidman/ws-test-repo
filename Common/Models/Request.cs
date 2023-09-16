using Common.Models.Requests.Abstract;

namespace Common.Models
{
    public class Request : IRequest
    {
        public RequestType RequestType { get; set; }
        public IRequestData RequestData { get; set; }
    }
}
