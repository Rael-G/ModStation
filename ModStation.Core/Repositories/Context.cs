using System.Data;
using System.Reflection;
using Lilmihe;
using Microsoft.Data.Sqlite;
using ModStation.Core.Interfaces;

namespace ModStation.Core.Repositories;

public class Context : IContext
{
    private readonly string _connectionString;

    private readonly string _scriptsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "./migrations");

    private static bool _migrated = false;

    public Context(string connectionString)
    {
        _connectionString = connectionString;
        _ = Migrate();
    }

    public IDbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    public IDbTransaction BeginTransaction(IDbConnection connection)
    {
        connection.Open();
        return connection.BeginTransaction();
    }

    private async Task Migrate()
    {
        using var connection = CreateConnection();

        connection.Open();

        var migrator = new MigrationHelper(_scriptsPath, connection);
        if(!_migrated)
        {
            var result = await migrator.Migrate();
            _migrated = true;

            if (!result.Success)
            {
                Console.WriteLine($"Migration failed: {result.Message}");
                if (result.Error != null)
                {
                    Console.WriteLine($"File: {result.Error.Message}");
                }
            }
        }
    }
}
