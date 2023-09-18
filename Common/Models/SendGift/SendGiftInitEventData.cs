namespace Common.Models.SendGift;

public record SendGiftInitEventData(Guid SenderId, Guid ReceiverId, ResourceType Resource, int Amount) : IEventData;