using Common.Models.Requests.UpdateResources;
using GameServer.Repositories.Models;

namespace GameServer.Repositories;

public interface IPlayersRepository
{
    // async and Tasks not required for memory storage, but keeping them for the future (probably switching to real DB)
    Task<bool> TryGetPlayer(Guid deviceId, out Player player);
    Task<Guid> AddPlayer(Guid deviceId);
    Task UpdateResources(Guid playerId, ResourceType resourceType, int amount);
    Task TransferResources(Player sender, Player receiver, ResourceType resource, int amount);
}