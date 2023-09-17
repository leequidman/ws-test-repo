using Common.Models.Requests.Abstract;
using Common.Models.Requests.UpdateResources;

namespace Common.Models.Requests.SendGift;

public record SendGiftInitEventData(Guid SenderId, Guid ReceiverId, ResourceType Resource, int Amount) : IEventData;