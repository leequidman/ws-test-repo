using System.Net.WebSockets;

namespace GameServer.Handlers;

public interface IBaseMessageHandler
{
    Task Handle(WebSocketReceiveResult webSocketReceiveResult, WebSocket ws, byte[] buffer);
}