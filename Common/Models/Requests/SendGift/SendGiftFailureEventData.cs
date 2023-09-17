using Common.Models.Requests.Abstract;

namespace Common.Models.Requests.SendGift;

public record SendGiftFailureEventData(string Message) : IEventData;