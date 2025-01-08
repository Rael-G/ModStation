using System.Data;
using System.Threading.Tasks;
using Dapper;
using ModManager.Core.Entities;
using ModStation.Core.Interfaces;

namespace ModManager.Core.Repositories;

public class ArchiveRepository(string connectionString) : BaseRepository(connectionString), IArchiveRepository
{
    public async Task CreateAsync(Archive archive)
    {
        using var connection = CreateConnection();
        await CreateAsync(archive, connection);
    }

    public async Task UpdateAsync(Archive archive)
    {
        using var connection = CreateConnection();
        await UpdateAsync(archive, connection);
    }

    public async Task DeleteAsync(Archive archive)
    {
        using var connection = CreateConnection();
        await DeleteAsync(archive, connection);
    }

    public async Task CreateAsync(Archive archive, IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = @"
            INSERT INTO Archives (
                Id, RelativePath, GameId
            )
            VALUES (
                @Id, @RelativePath, @GameId
            )

        ";

        await connection.ExecuteAsync(sql, archive, transaction);
    }

    public async Task UpdateAsync(Archive archive, IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = @"
            UPDATE Archives
            SET RelativePath = @RelativePath
            WHERE Id = @Id;
        ";
        await connection.ExecuteAsync(sql, archive, transaction);
    }

    public async Task DeleteAsync(Archive archive, IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = @"
            DELETE FROM Archives WHERE Id = @Id;
        ";
        await connection.ExecuteAsync(sql, new { archive.Id}, transaction);
    }
}
