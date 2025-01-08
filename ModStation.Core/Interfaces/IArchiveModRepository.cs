using System.Data;

namespace ModStation.Core.Interfaces;

public interface IArchiveModRepository : IBaseRepository
{
    void Create(string archiveId, string modId);
    void Delete(string archiveId, string modId);
    void Create(string archiveId, string modId, IDbConnection connection, IDbTransaction? transaction = null);
    void Delete(string archiveId, string modId, IDbConnection connection, IDbTransaction? transaction = null);
}
