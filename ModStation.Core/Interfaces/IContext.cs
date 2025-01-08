using System.Data;

namespace ModStation.Core.Interfaces;

public interface IContext
{
    IDbConnection CreateConnection();
    IDbTransaction BeginTransaction(IDbConnection connection);
}
