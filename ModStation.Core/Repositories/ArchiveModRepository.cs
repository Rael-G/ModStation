using System.Data;
using Dapper;
using ModStation.Core.Interfaces;

namespace ModStation.Core.Repositories;

public class ArchiveModRepository(IContext context) : BaseRepository(context), IArchiveModRepository
{
    public async Task CreateAsync(string archiveId, string modId)
    {
        using var connection = Context.CreateConnection();
        await CreateAsync(archiveId, modId, connection);
    }

    public async Task DeleteAsync(string archiveId, string modId)
    {
        using var connection = Context.CreateConnection();
        await DeleteAsync(archiveId, modId, connection);
    }

    public async Task CreateAsync(string archiveId, string modId, IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = @"
            INSERT INTO ArchiveMod (
                ArchiveId, ModId
            )
            VALUES (
                @ArchiveId, @ModId
            )

        ";

        await connection.ExecuteAsync(sql, new { ArchiveId = archiveId, ModId = modId }, transaction);
    }

    public async Task DeleteAsync(string archiveId, string modId, IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = @"
            DELETE FROM ArchiveMod WHERE ArchiveId = @ArchiveId AND ModId = @ModId;
        ";
        await connection.ExecuteAsync(sql, new { ArchiveId = archiveId, ModId = modId }, transaction);
    }

}
