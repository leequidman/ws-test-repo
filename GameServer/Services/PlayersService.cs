using Common.Models.Requests.UpdateResources;
using GameServer.Repositories;

namespace GameServer.Services;

public interface IPlayersService
{
    Task<Guid> GetOrAddPlayerId(Guid deviceId);
    Task UpdateResources(Guid playerId, ResourceType resourceType, int amount);
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
        if (await _playersRepository.TryGetPlayer(deviceId, out var player))
            return player.PlayerId;

        return await _playersRepository.AddPlayer(deviceId);        
    }

    public async Task UpdateResources(Guid playerId, ResourceType resourceType, int amount)
    {
        await _playersRepository.UpdateResources(playerId, resourceType, amount);
    }
}