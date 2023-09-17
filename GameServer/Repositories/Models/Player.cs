using System.Collections.Concurrent;
using Common.Models;

namespace GameServer.Repositories.Models;

public class Player
{
    public Player(Guid playerId, Guid deviceId)
    {
        PlayerId = playerId;
        DeviceId = deviceId;
    }

    public Guid PlayerId { get; init; }
    public Guid DeviceId { get; init; }
    public ConcurrentDictionary<ResourceType, int> Resources { get; init; } = new();
}