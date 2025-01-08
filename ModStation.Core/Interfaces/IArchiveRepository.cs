using System.Data;
using ModManager.Core.Entities;

namespace ModStation.Core.Interfaces;

public interface IArchiveRepository : IBaseRepository
{
    Task CreateAsync(Archive archive);
    Task UpdateAsync(Archive archive);
    Task DeleteAsync(Archive archive);
    Task CreateAsync(Archive archive, IDbConnection connection, IDbTransaction? transaction = null);
    Task UpdateAsync(Archive archive, IDbConnection connection, IDbTransaction transaction);
    Task DeleteAsync(Archive archive, IDbConnection connection, IDbTransaction? transaction = null);
}
