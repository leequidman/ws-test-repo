using Common.Models;

namespace GameServer.Transport;

public interface IEventHandlerProvider
{
    IEnumerable<IEventHandler> GetHandlers(EventType eventType);
}

public class EventHandlerProvider : IEventHandlerProvider
{
    private readonly IServiceProvider _serviceProvider;

    public EventHandlerProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IEnumerable<IEventHandler> GetHandlers(EventType eventType)
    {
        var allEventHandlers = _serviceProvider.GetServices<IEventHandler>();
        if (allEventHandlers == null)
            throw new ArgumentException("No handlers found. Smth must be wrong with DI registration");

        return allEventHandlers.Where(s => s.EventType == eventType);
    }
}