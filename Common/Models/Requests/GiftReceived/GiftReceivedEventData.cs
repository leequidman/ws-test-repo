using Common.Models.Requests.Abstract;
using Common.Models.Requests.UpdateResources;

namespace Common.Models.Requests.GiftReceived;

public record GiftReceivedEventData(Guid SenderId, Guid ReceiverId, ResourceType Resource, int AmountSent, int ReceiverNewAmount) : IEventData;