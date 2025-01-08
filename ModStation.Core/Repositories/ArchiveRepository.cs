using System.Data;
using Dapper;
using ModManager.Core.Entities;
using ModStation.Core.Interfaces;

namespace ModManager.Core.Repositories;

public class ArchiveRepository(string connectionString) : BaseRepository(connectionString), IArchiveRepository
{
    public void Create(Archive archive)
    {
        using var connection = CreateConnection();
        Create(archive, connection);
    }

    public void Update(Archive archive)
    {
        using var connection = CreateConnection();
        Update(archive, connection);
    }

    public void Delete(Archive archive)
    {
        using var connection = CreateConnection();
        Delete(archive, connection);
    }

    public void Create(Archive archive, IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = @"
            INSERT INTO Archives (
                Id, RelativePath, GameId
            )
            VALUES (
                @Id, @RelativePath, @GameId
            )

        ";

        connection.Execute(sql, archive, transaction);
    }

    public void Update(Archive archive, IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = @"
            UPDATE Archives
            SET RelativePath = @RelativePath
            WHERE Id = @Id;
        ";
        connection.Execute(sql, archive, transaction);
    }

    public void Delete(Archive archive, IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = @"
            DELETE FROM Archives WHERE Id = @Id;
        ";
        connection.Execute(sql, new { archive.Id}, transaction);
    }
}
