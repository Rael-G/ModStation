using System.Data;
using System.Reflection;
using Lilmihe;
using Microsoft.Data.Sqlite;

namespace ModManager.Core.Repositories;

public class BaseRepository
{
    public string ConnectionString { get; set; }

    private readonly string _scriptsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "./migrations");

    private static bool _migrated = false;

    public BaseRepository(string connectionString)
    {
        ConnectionString = connectionString;
        using var connection = CreateConnection();

        connection.Open();

        var migrator = new MigrationHelper(_scriptsPath, connection);
        if(!_migrated)
        {
            var migration = migrator.Migrate();
            migration.Wait();
            var result = migration.Result;
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

    public IDbConnection CreateConnection()
    {
        return new SqliteConnection(ConnectionString);
    }
}
