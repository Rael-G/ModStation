using System.Data;
using ModManager.Core.Entities;

namespace ModStation.Core.Interfaces;

public interface IModRepository : IBaseRepository
{
    void Create(Mod mod);
    void Update(Mod mod);
    void Delete(Mod mod);
    void Create(Mod mod, IDbConnection connection, IDbTransaction? transaction = null);
    void Update(Mod mod, IDbConnection connection, IDbTransaction transaction);
    void Delete(Mod mod, IDbConnection connection, IDbTransaction? transaction = null);
}
