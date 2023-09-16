using Common.Models;

namespace GameServer.Handlers;

public interface IEventHandlerProvider
{
    IEnumerable<IEventHandler> GetHandlers(EventType eventType);
}