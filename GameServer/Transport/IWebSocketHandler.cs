using System.Net.WebSockets;
using Common.Models.Requests.Abstract;

namespace Common.Transport;

public interface IWebSocketHandler
{
    Task ReceiveMessage(WebSocket ws);
    Task SendEvent(WebSocket ws, IEvent @event);
}