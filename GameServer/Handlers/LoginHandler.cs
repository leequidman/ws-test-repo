using System.Net.WebSockets;
using Common.Models.Response.Login;
using GameServer.Client;
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
        private readonly ISender _sender;

        public LoginHandler(IPlayersService playersService, IConnectionService connectionService, ISender sender)
        {
            _playersService = playersService;
            _connectionService = connectionService;
            _sender = sender;
        }

        public async Task Handle(Guid deviceId, WebSocket ws)
        {
            var playerId = await _playersService.GetOrAddPlayerId(deviceId);
            LoginResponse loginResponse;
            if (_connectionService.IsOnline(playerId))
            {
                // mb go with silent success instead?
                loginResponse = new($"Player {playerId} is already online");
            }
            else
            {
                _connectionService.SetOnline(playerId, ws);
                loginResponse = new(new LoginResponseResult(playerId));
            }
            await _sender.Send(ws, loginResponse);
        }
    }
}
