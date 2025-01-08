using System.Data;
using ModManager.Core.Entities;

namespace ModStation.Core.Interfaces;

public interface IModRepository : IBaseRepository
{
    Task CreateAsync(Mod mod);
    Task UpdateAsync(Mod mod);
    Task DeleteAsync(Mod mod);
    Task CreateAsync(Mod mod, IDbConnection connection, IDbTransaction? transaction = null);
    Task UpdateAsync(Mod mod, IDbConnection connection, IDbTransaction transaction);
    Task DeleteAsync(Mod mod, IDbConnection connection, IDbTransaction? transaction = null);
}
