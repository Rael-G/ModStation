using System.Data;
using ModManager.Core.Entities;

namespace ModStation.Core.Interfaces;

public interface IGameRepository : IBaseRepository
{
    Task CreateAsync(Game game);
    Task UpdateAsync(Game game);
    Task DeleteAsync(Game game);
    Task<IEnumerable<Game>> GetAllAsync();
    Task CreateAsync(Game game, IDbConnection connection, IDbTransaction? transaction = null);
    Task UpdateAsync(Game game, IDbConnection connection, IDbTransaction transaction);
    Task DeleteAsync(Game game, IDbConnection connection, IDbTransaction? transaction = null);
}
