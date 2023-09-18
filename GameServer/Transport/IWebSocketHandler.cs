using System.Net.WebSockets;
using Common.Models;

namespace GameServer.Transport;

public interface IWebSocketHandler
{
    Task ReceiveMessage(WebSocket ws);
    Task SendEvent(WebSocket ws, IEvent @event);
}