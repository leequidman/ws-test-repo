using GameServer.Repositories;

namespace GameServer.Services;

public interface IPlayersService
{
    Task<Guid> GetOrAddPlayerId(Guid deviceId);
}

public class PlayersService : IPlayersService
{
    private readonly IPlayersRepository _playersRepository;

    public PlayersService(IPlayersRepository playersRepository)
    {
        _playersRepository = playersRepository;
    }
    public async Task<Guid> GetOrAddPlayerId(Guid deviceId)
    {
        if (await _playersRepository.TryGetPlayerId(deviceId, out var playerId))
            return playerId;

        return await _playersRepository.AddPlayer(deviceId);        
    }
}