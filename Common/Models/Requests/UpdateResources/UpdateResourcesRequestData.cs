using Common.Models.Requests.Abstract;

namespace Common.Models.Requests.UpdateResources;

public class UpdateResourcesRequestData : IRequestData
{
    public ResourceType ResourceType { get; set; }
    public int Amount { get; set; }
}