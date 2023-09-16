using Common.Models.Requests.Abstract;

namespace Common.Models;

public interface IRequestHandler
{
    EventType EventType { get; }

    Task<IEvent> Handle(IEventData eventData);
}