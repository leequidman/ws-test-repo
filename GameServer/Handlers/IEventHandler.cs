using System.Net.WebSockets;
using Common.Models;

namespace GameServer.Handlers;

public interface IEventHandler
{
    public EventType EventType { get; }
    public Task Handle(object? eventData, WebSocket ws);

}