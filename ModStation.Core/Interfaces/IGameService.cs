using ModManager.Core.Entities;

namespace ModStation.Core.Interfaces;

public interface IGameService
{
    Task<Game> CreateAsync(string gamePath, string name);
    Task DeleteAsync(Game game);
}
