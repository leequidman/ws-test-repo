using System.Collections.Concurrent;
using Common.Models.Requests.UpdateResources;
using GameServer.Repositories.Models;

namespace GameServer.Repositories;

public class PlayersRepository : IPlayersRepository
{
    private readonly Serilog.ILogger _logger;
    private static readonly ConcurrentDictionary<Guid, Player> Players = new();

    public PlayersRepository(Serilog.ILogger logger)
    {
        _logger = logger;
    }

    public Task<bool> TryGetPlayer(Guid deviceId, out Player player)
    {
        return Task.FromResult(Players.TryGetValue(deviceId, out player));
    }

    public Task<Guid> AddPlayer(Guid deviceId)
    {
        var player = new Player(Guid.NewGuid(), deviceId);

        if (Players.TryAdd(deviceId, player))
            return Task.FromResult(player.PlayerId);

        // this deviceId is already in dictionary
        if (Players.TryGetValue(deviceId, out player))
            return Task.FromResult(player.PlayerId);
        throw new($"Failed to add playerId for deviceId '{deviceId}' (meaning it is already there), " +
                  "but also failed to get it. Wtf?");
    }

    public Task UpdateResources(Guid playerId, ResourceType resourceType, int amount)
    {
        if (!Players.TryGetValue(playerId, out var player))
            throw new($"Failed to update resources for playerId '{playerId}' because it does not exist");

        player.Resources.AddOrUpdate(resourceType, amount, (_, _) => amount);
        return Task.CompletedTask;
    }

    public Task TransferResources(Player sender, Player receiver, ResourceType resource, int amount)
    {
        sender.Resources.AddOrUpdate(resource, 0, (key, oldValue) => oldValue - amount);
        receiver.Resources.AddOrUpdate(resource, amount, (key, oldValue) => oldValue + amount);
        return Task.CompletedTask;
    }
}