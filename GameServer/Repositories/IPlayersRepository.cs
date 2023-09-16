namespace GameServer.Repositories;

public interface IPlayersRepository
{
    // async and Tasks not required for memory storage, but keeping them for the future (probably switching to real DB)
    Task<bool> TryGetPlayerId(Guid deviceId, out Guid playerId);
    Task<Guid> AddPlayer(Guid deviceId);
}