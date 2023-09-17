using Common.Models;
using GameServer.Repositories;
using GameServer.Repositories.Models;

namespace GameServer.Services;

public interface IPlayersService
{
    Task<Guid> GetOrAddPlayerId(Guid deviceId);
    Task UpdateResources(Guid playerId, ResourceType resourceType, int amount);
    Task<Player?> FindPlayer(Guid playerId);
    Task TransferResources(Player sender, Player receiver, ResourceType dataResource, int dataAmount);
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
            return player!.PlayerId;

        return await _playersRepository.AddPlayer(deviceId);        
    }

    public async Task UpdateResources(Guid playerId, ResourceType resourceType, int amount)
    {
        await _playersRepository.UpdateResources(playerId, resourceType, amount);
    }

    public async Task<Player?> FindPlayer(Guid playerId)
    {
        var playerDeviceId = await _playersRepository.GetPlayerDeviceId(playerId);      
        
        if (await _playersRepository.TryGetPlayer(playerDeviceId, out var player))
            return player;
        
        return null;
    }

    public async Task TransferResources(Player sender, Player receiver, ResourceType resource, int amount)
    {
        await _playersRepository.TransferResources(sender, receiver, resource, amount);
    }
}