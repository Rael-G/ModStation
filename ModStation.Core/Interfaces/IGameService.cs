using ModManager.Core.Entities;

namespace ModStation.Core.Interfaces;

public interface IGameService
{
    Task<IEnumerable<Game>> GetAllAsync();
    Task<Game> CreateAsync(string gamePath, string name);
    Task DeleteAsync(Game game);
    Task UpdateAsync(Game game);
}
