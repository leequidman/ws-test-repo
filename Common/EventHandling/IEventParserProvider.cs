using Common.Models;

namespace Common.EventHandling;

public interface IEventParserProvider
{
    IEventParser GetParser(EventType eventType);
}