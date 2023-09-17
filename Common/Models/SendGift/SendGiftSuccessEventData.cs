using Common.Models.UpdateResources;

namespace Common.Models.SendGift;

public record SendGiftSuccessEventData(
    Guid SenderId,
    Guid ReceiverId,
    ResourceType Resource,
    int SenderCurrentAmount,
    int ReceiverCurrentAmount) : IEventData;