using Common.Models;

namespace GameServer.Transport;

public class EventParserProvider : IEventParserProvider
{
    private readonly IServiceProvider _serviceProvider;

    public EventParserProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IEventParser GetParser(EventType eventType)
    {
        var allEventParsers = _serviceProvider.GetServices<IEventParser>();
        if (allEventParsers == null)
            throw new ArgumentException("No parsers found. Smth must be wrong with DI registration");

        var allEventParsersList = allEventParsers.Where(p => p.EventType == eventType).ToList();
        if (allEventParsersList.Count != 1)
            throw new ArgumentException("Only one parser is allowed. Smth must be wrong with DI registration");

        return allEventParsersList.Single();
    }
}