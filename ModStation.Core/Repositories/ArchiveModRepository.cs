using System.Data;
using Dapper;

namespace ModManager.Core.Repositories;

public class ArchiveModRepository(string connectionString) : BaseRepository(connectionString)
{
    public void Create(string archiveId, string modId)
    {
        using var connection = CreateConnection();
        Create(archiveId, modId, connection);
    }

    public void Delete(string archiveId, string modId)
    {
        using var connection = CreateConnection();
        Delete(archiveId, modId, connection);
    }

    public void Create(string archiveId, string modId, IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = @"
            INSERT INTO ArchiveMod (
                ArchiveId, ModId
            )
            VALUES (
                @ArchiveId, @ModId
            )

        ";

        connection.Execute(sql, new { ArchiveId = archiveId, ModId = modId }, transaction);
    }

    public void Delete(string archiveId, string modId, IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = @"
            DELETE FROM ArchiveMod WHERE ArchiveId = @ArchiveId AND ModId = @ModId;
        ";
        connection.Execute(sql, new { ArchiveId = archiveId, ModId = modId }, transaction);
    }

}
