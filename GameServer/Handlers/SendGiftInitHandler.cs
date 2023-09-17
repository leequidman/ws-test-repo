using System.Net.WebSockets;
using Common.EventHandling;
using Common.Models;

namespace GameServer.Handlers;

public class SendGiftInitHandler : IEventHandler
{
    public EventType EventType => EventType.SendGiftInit;

    public Task Handle(object? eventData, WebSocket ws)
    {

        
    }
}