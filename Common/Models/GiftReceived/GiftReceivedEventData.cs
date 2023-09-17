namespace Common.Models.GiftReceived;

public record GiftReceivedEventData(Guid SenderId, Guid ReceiverId, ResourceType Resource, int AmountSent, int ReceiverNewAmount) : IEventData;