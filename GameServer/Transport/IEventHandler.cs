using System.Net.WebSockets;
using Common.Models;

namespace GameServer.Transport;

public interface IEventHandler
{
    public EventType EventType { get; }
    public Task Handle(object? eventData, WebSocket ws);
}