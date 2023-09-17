using System.Net.WebSockets;
using Common.EventHandling;
using Common.Models;
using Common.Models.Requests.Abstract;
using Common.Models.Requests.Login;
using Common.Transport;
using GameServer.EventDataProcessing;
using GameServer.Services;
using JetBrains.Annotations;

namespace GameServer.Handlers;

[UsedImplicitly]
public class LoginInitHandler : IEventHandler
{
    private readonly IPlayersService _playersService;
    private readonly IConnectionService _connectionService;
    private readonly IWebSocketHandler _webSocketHandler;
    private readonly LoginInitEventDataProcessor _dataProcessor = new();
    private readonly Serilog.ILogger _logger;

    public EventType EventType => EventType.LoginInit;

    public LoginInitHandler(
        IPlayersService playersService,
        IConnectionService connectionService, 
        IWebSocketHandler webSocketHandler,
        Serilog.ILogger logger)
    {
        _playersService = playersService;
        _connectionService = connectionService;
        _webSocketHandler = webSocketHandler;
        _logger = logger;
    }

    public async Task Handle(object? eventData, WebSocket ws)
    {
        var data = _dataProcessor.PrepareEventData(eventData);

        var playerId = await _playersService.GetOrAddPlayerId(data.DeviceId);

        IEvent loginEvent;
        try
        {
            if (_connectionService.IsOnline(playerId)) // mb go with silent success instead?
            {
                loginEvent = new LoginFailedEvent(new($"Player {playerId} is already online"));
            }
            else
            {
                _connectionService.SetOnline(playerId, ws);
                loginEvent = new LoginSuccessfulEvent(new(playerId));
            }
        }
        catch (Exception e)
        {
            _logger.Error(e, $"Failed to login by device id '{data.DeviceId}'.");
            loginEvent = new LoginFailedEvent(new($"Failed to login by device id '{data.DeviceId}': {e.Message}"));
        }

        await _webSocketHandler.SendEvent(ws, loginEvent);
    }
}