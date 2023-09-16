namespace Common.Models.Requests.UpdateResources;

public class UpdateResourceRequest
{
    public RequestType RequestType => RequestType.UpdateResources;
    public UpdateResourcesRequestData RequestData { get; }

    public UpdateResourceRequest(UpdateResourcesRequestData requestData)
    {
        RequestData = requestData;
    }
}