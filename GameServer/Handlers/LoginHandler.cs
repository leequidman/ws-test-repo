using System.Net.WebSockets;
using Common.Models.Requests.Abstract;
using Common.Models.Requests.Login;
using Common.Services;
using GameServer.Services;

namespace GameServer.Handlers
{
    public interface ILoginHandler
    {
        Task Handle(Guid deviceId, WebSocket ws);
    }

    public class LoginHandler : ILoginHandler
    {
        private readonly IPlayersService _playersService;
        private readonly IConnectionService _connectionService;  
        private readonly IEventSender _eventSender;

        public LoginHandler(IPlayersService playersService, IConnectionService connectionService, IEventSender eventSender)
        {
            _playersService = playersService;
            _connectionService = connectionService;
            _eventSender = eventSender;
        }

        public async Task Handle(Guid deviceId, WebSocket ws)
        {
            var playerId = await _playersService.GetOrAddPlayerId(deviceId);

            IEvent loginEvent;
            if (_connectionService.IsOnline(playerId)) {
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
