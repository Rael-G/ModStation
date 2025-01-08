using System;
using System.Data;

namespace ModStation.Core.Interfaces;

public interface IBaseRepository
{
    IDbConnection CreateConnection();
    IDbTransaction CreateTransaction(IDbConnection connection);
}
