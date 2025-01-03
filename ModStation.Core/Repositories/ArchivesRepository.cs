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
}
