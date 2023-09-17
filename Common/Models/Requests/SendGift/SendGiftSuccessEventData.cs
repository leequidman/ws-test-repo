using Common.Models.Requests.Abstract;
using Common.Models.Requests.UpdateResources;

namespace Common.Models.Requests.SendGift;

public record SendGiftSuccessEventData(
    Guid SenderId,
    Guid ReceiverId,
    ResourceType Resource,
    int SenderCurrentAmount,
    int ReceiverCurrentAmount) : IEventData;