using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace GameServer.Services;

public interface IConnectionService
{
    void SetOnline(Guid playerId, WebSocket webSocket);
    void SetOffline(Guid playerId);
    bool IsOnline(Guid playerId);
    WebSocket? GetWebSocket(Guid playerId);
}

public class ConnectionService : IConnectionService
{
    private readonly ConcurrentDictionary<Guid, WebSocket> _activePlayers = new();
    private readonly Serilog.ILogger _logger;

    public ConnectionService(Serilog.ILogger logger)
    {
        _logger = logger;
    }

    public bool IsOnline(Guid playerId)
    {
        if (_activePlayers.TryGetValue(playerId, out var ws))
        {
            if (ws.State == WebSocketState.Open)
                return true;

            _logger.Warning($"For some reason there was a not open WS connection kept for active player '{playerId}'. " +
                            "WS info: " +
                            $"State: {ws.State}, " +
                            $"CloseStatus: {ws.CloseStatus}, " +
                            $"CloseStatusDescription: {ws.CloseStatusDescription}");
            _activePlayers.TryRemove(playerId, out _);
            return false;

        }

        return false;
    }

    public WebSocket? GetWebSocket(Guid playerId)
    {
        if (IsOnline(playerId))
            if (_activePlayers.TryGetValue(playerId, out var ws))
                return ws;

        return null;

    }

    public void SetOnline(Guid playerId, WebSocket webSocket)
    {
        _activePlayers.TryAdd(playerId, webSocket);
        _logger.Information($"Player {playerId} connected");

    }

    public void SetOffline(Guid playerId)
    {
        _activePlayers.TryRemove(playerId, out _);
        _logger.Information($"Player {playerId} disconnected");  
    }

        
}