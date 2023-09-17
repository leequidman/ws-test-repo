using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace GameServer.Services;

public interface IConnectionService
{
    void SetOnline(Guid playerId, WebSocket webSocket);
    void SetOffline(WebSocket ws);
    bool IsOnline(Guid playerId);
    WebSocket? GetWebSocket(Guid playerId);
}

public class ConnectionService : IConnectionService
{
    // double dictionary for fast access to ws by player id and vice versa
    // on smaller scale it's not a problem to keep it in memory
    private readonly ConcurrentDictionary<Guid, WebSocket> _playerIdToWs = new();
    private readonly ConcurrentDictionary<WebSocket, Guid> _wsToPlayerId = new();
    private readonly Serilog.ILogger _logger;

    public ConnectionService(Serilog.ILogger logger)
    {
        _logger = logger;
    }

    public bool IsOnline(Guid playerId)
    {
        if (_playerIdToWs.TryGetValue(playerId, out var ws))
        {
            if (ws.State == WebSocketState.Open)
                return true;

            _logger.Warning($"For some reason there was a not open WS connection kept for active player '{playerId}'. " +
                            "WS info: " +
                            $"State: {ws.State}, " +
                            $"CloseStatus: {ws.CloseStatus}, " +
                            $"CloseStatusDescription: {ws.CloseStatusDescription}");
            _playerIdToWs.TryRemove(playerId, out _);
            _wsToPlayerId.TryRemove(ws, out _);
            return false;

        }

        return false;
    }

    public WebSocket? GetWebSocket(Guid playerId)
    {
        if (IsOnline(playerId))
            if (_playerIdToWs.TryGetValue(playerId, out var ws))
                return ws;

        return null;

    }

    public void SetOnline(Guid playerId, WebSocket webSocket)
    {
        _playerIdToWs.TryAdd(playerId, webSocket);
        _wsToPlayerId.TryAdd(webSocket, playerId);
        _logger.Warning($"Player {playerId} connected");

    }

    public void SetOffline(WebSocket ws)
    {
        _wsToPlayerId.TryRemove(ws, out var playerId);
        if (playerId != Guid.Empty)
            _playerIdToWs.TryRemove(playerId, out _);

        _logger.Warning($"Player {playerId} disconnected");  
    }

        
}