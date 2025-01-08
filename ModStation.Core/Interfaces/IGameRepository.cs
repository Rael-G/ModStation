using System.Data;
using ModManager.Core.Entities;

namespace ModStation.Core.Interfaces;

public interface IGameRepository : IBaseRepository
{
    void Create(Game game);
    void Update(Game game);
    void Delete(Game game);
    IEnumerable<Game> GetAll();
    void Create(Game game, IDbConnection connection, IDbTransaction? transaction = null);
    void Update(Game game, IDbConnection connection, IDbTransaction transaction);
    void Delete(Game game, IDbConnection connection, IDbTransaction? transaction = null);
}
