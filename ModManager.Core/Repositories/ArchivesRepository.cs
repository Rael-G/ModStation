using Dapper;
using ModManager.Core.Entities;

namespace ModManager.Core.Repositories;

public class ArchivesRepository(string connectionString) : BaseRepository(connectionString)
{
    public void Create(Archive archive)
    {
        using var connection = CreateConnection();
        var sql = @"
            INSERT INTO Archives (
                Id, RelativePath, GameId
            )
            VALUES (
                @Id, @RelativePath, @GameId
            )

        ";

        connection.Execute(sql, archive);
    }

    public void Update(Archive archive)
    {
        using var connection = CreateConnection();
        var sql = @"
            UPDATE Archives
            SET RelativePath = @RelativePath
            WHERE Id = @Id;
        ";
        connection.Execute(sql, archive);
    }

    public void Delete(Archive archive)
    {
        using var connection = CreateConnection();
        var sql = @"
            DELETE FROM Archives WHERE Id = @Id;
        ";
        connection.Execute(sql, new { archive.Id});
    }

    public Archive? GetById(Guid id)
    {
        using var connection = CreateConnection();
        var sql = @"
            SELECT * FROM Archives WHERE Id = @Id;
        ";
        var archive = connection.Query<Archive>(sql, new { Id = id }).FirstOrDefault();

        if (archive != null)
        {
            var modsSql = @"
                SELECT m.* 
                FROM Mods m
                INNER JOIN ArchiveMod am ON m.Id = am.ModId
                WHERE am.ArchiveId = @ArchiveId;
            ";
            archive.Mods = connection.Query<Mod>(modsSql, new { ArchiveId = archive.Id }).ToList();
        }

        return archive;
    }

    public Archive? GetByRelativePath(string relativePath)
    {
        using var connection = CreateConnection();
        var sql = @"
            SELECT * FROM Archives WHERE RelativePath = @RelativePath;
        ";
        var archive = connection.Query<Archive>(sql, new { RelativePath = relativePath }).FirstOrDefault();

        if (archive != null)
        {
            var modsSql = @"
                SELECT m.* 
                FROM Mods m
                INNER JOIN ArchiveMod am ON m.Id = am.ModId
                WHERE am.ArchiveId = @ArchiveId;
            ";
            archive.Mods = connection.Query<Mod>(modsSql, new { ArchiveId = archive.Id }).ToList();
        }

        return archive;
    }
}
