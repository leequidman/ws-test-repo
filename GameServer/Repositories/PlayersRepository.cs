using System.Collections.Concurrent;

namespace GameServer.Repositories
{
    public class PlayersRepository : IPlayersRepository
    {
        private readonly Serilog.ILogger _logger;
        private static readonly ConcurrentDictionary<Guid, Guid> Players = new();

        public PlayersRepository(Serilog.ILogger logger)
        {
            _logger = logger;
        }

        public Task<bool> TryGetPlayerId(Guid deviceId, out Guid playerId)
        {
            return Task.FromResult(Players.TryGetValue(deviceId, out playerId));
        }

        public Task<Guid> AddPlayer(Guid deviceId)
        {
            var playerId = Guid.NewGuid();
            if (Players.TryAdd(deviceId, playerId))
                return Task.FromResult(playerId);

            // this deviceId is already in dictionary
            if (Players.TryGetValue(deviceId, out playerId))
                return Task.FromResult(playerId);
            throw new($"Failed to add playerId for deviceId '{deviceId}' (meaning it is already there), " +
                      "but also failed to get it. Wtf?");
        }
    }
}
