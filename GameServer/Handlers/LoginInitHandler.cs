using System.Net.WebSockets;
using Common.EventHandling;
using Common.Models;
using Common.Models.Requests.Abstract;
using Common.Models.Requests.Login;
using Common.Transport;
using GameServer.Services;
using JetBrains.Annotations;

namespace GameServer.Handlers
{
    [UsedImplicitly]
    public class LoginInitHandler : IEventHandler
    {
        private readonly IPlayersService _playersService;
        private readonly IConnectionService _connectionService;
        private readonly IWebSocketHandler _webSocketHandler;

        public EventType EventType => EventType.LoginInit;

        public LoginInitHandler(IPlayersService playersService, IConnectionService connectionService, IWebSocketHandler webSocketHandler)
        {
            _playersService = playersService;
            _connectionService = connectionService;
            _webSocketHandler = webSocketHandler;
        }

        public async Task Handle(object? eventData, WebSocket ws)
        {
            if (eventData is null)
                throw new ArgumentException($"Expected {nameof(eventData)} to be non-null");

            if (eventData is not LoginInitEventData initLoginEventData)
                throw new ArgumentException($"Expected {nameof(LoginInitEventData)} but got {eventData.GetType().Name}");

            if (initLoginEventData == null)
                throw new ArgumentException($"Expected {nameof(initLoginEventData)} to be non-null");

            if (initLoginEventData.DeviceId == Guid.Empty)
                throw new ArgumentException($"Expected {nameof(initLoginEventData.DeviceId)} to be non-empty");

            var playerId = await _playersService.GetOrAddPlayerId(initLoginEventData.DeviceId);

            IEvent loginEvent;
            if (_connectionService.IsOnline(playerId))
            {
                // mb go with silent success instead?
                loginEvent = new LoginFailedEvent(new($"Player {playerId} is already online"));
            }
            else
            {
                _connectionService.SetOnline(playerId, ws);
                loginEvent = new LoginSuccessfulEvent(new(playerId));
            }

            await _webSocketHandler.SendEvent(ws, loginEvent);
        }
    }
}