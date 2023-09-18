using Common.Models;

namespace GameServer.Transport;

public interface IEventParserProvider
{
    IEventParser GetParser(EventType eventType);
}