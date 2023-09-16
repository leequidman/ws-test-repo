using System.Net.WebSockets;
using Common.Models;
using Common.Models.Requests.Abstract;
using Common.Models.Requests.Login;
using Common.Services;
using GameServer.Services;
using JetBrains.Annotations;

namespace GameServer.Handlers
{
    [UsedImplicitly]
    public class InitLoginHandler : IEventHandler
    {
        private readonly IPlayersService _playersService;
        private readonly IConnectionService _connectionService;
        private readonly IEventSender _eventSender;

        public EventType EventType => EventType.InitLogin;

        public InitLoginHandler(IPlayersService playersService, IConnectionService connectionService, IEventSender eventSender)
        {
            _playersService = playersService;
            _connectionService = connectionService;
            _eventSender = eventSender;
        }

        public async Task Handle(object? eventData, WebSocket ws)
        {
            if (eventData is null)
                throw new ArgumentException($"Expected {nameof(eventData)} to be non-null");

            if (eventData is not InitLoginEventData initLoginEventData)
                throw new ArgumentException($"Expected {nameof(InitLoginEventData)} but got {eventData.GetType().Name}");

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

            await _eventSender.Send(ws, loginEvent);
        }
    }
}