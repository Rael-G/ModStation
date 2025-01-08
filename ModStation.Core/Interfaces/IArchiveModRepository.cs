using System.Data;

namespace ModStation.Core.Interfaces;

public interface IArchiveModRepository : IBaseRepository
{
    Task CreateAsync(string archiveId, string modId);
    Task DeleteAsync(string archiveId, string modId);
    Task CreateAsync(string archiveId, string modId, IDbConnection connection, IDbTransaction? transaction = null);
    Task DeleteAsync(string archiveId, string modId, IDbConnection connection, IDbTransaction? transaction = null);
}
