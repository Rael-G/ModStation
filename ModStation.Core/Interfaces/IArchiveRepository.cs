using System.Data;
using ModManager.Core.Entities;

namespace ModStation.Core.Interfaces;

public interface IArchiveRepository : IBaseRepository
{
    void Create(Archive archive);
    void Update(Archive archive);
    void Delete(Archive archive);
    void Create(Archive archive, IDbConnection connection, IDbTransaction? transaction = null);
    void Update(Archive archive, IDbConnection connection, IDbTransaction transaction);
    void Delete(Archive archive, IDbConnection connection, IDbTransaction? transaction = null);
}
