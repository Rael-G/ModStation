using Dapper;

namespace ModManager.Core.Repositories;

public class ArchiveModRepository(string connectionString) : BaseRepository(connectionString)
{
    public void Create(string archiveId, string modId)
    {
        using var connection = CreateConnection();
        var sql = @"
            INSERT INTO ArchiveMod (
                ArchiveId, ModId
            )
            VALUES (
                @ArchiveId, @ModId
            )

        ";

        connection.Execute(sql, new { ArchiveId = archiveId, ModId = modId });
    }

    public void Delete(string archiveId, string modId)
    {
        using var connection = CreateConnection();
        var sql = @"
            DELETE FROM ArchiveMod WHERE ArchiveId = @ArchiveId AND ModId = @ModId;
        ";
        connection.Execute(sql, new { ArchiveId = archiveId, ModId = modId });
    }

}
