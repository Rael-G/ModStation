using Dapper;
using ModManager.Core.Entities;

namespace ModManager.Core.Repositories;

public class ModsRepository(string connectionString) : BaseRepository(connectionString)
{
    public void Create(Mod mod)
    {
        using var connection = CreateConnection();
        var sql = @"
            INSERT INTO Mods (
                Id, Name, ModPath, IsEnable, ""Order"", GameId
            )
            VALUES (
                @Id, @Name, @ModPath, @IsEnable, @Order, @GameId
            )

        ";

        connection.Execute(sql, mod);
    }

    public void Update(Mod mod)
    {
        using var connection = CreateConnection();
        var sql = @"
            UPDATE Mods
            SET Name = @Name, ModPath = @ModPath, ""Order"" = @Order, IsEnable = @IsEnable
            WHERE Id = @Id;
        ";
        connection.Execute(sql, mod);
    }

    public void Delete(Mod mod)
    {
        using var connection = CreateConnection();
        var sql = @"
            DELETE FROM Mods WHERE Id = @Id;
        ";
        connection.Execute(sql, new { mod.Id});
    }

    public Mod? GetById(Guid id)
    {
        using var connection = CreateConnection();
        var sql = @"
            SELECT * FROM Mods WHERE Id = @Id;
        ";
        var mod = connection.Query<Mod>(sql, new { Id = id }).FirstOrDefault();

        if (mod != null)
        {
            sql = @"
                SELECT a.* 
                FROM Archives a
                INNER JOIN ArchiveMod am ON a.Id = am.ArchiveId
                WHERE am.ModId = @ModId;
            ";
            mod.Archives = connection.Query<Archive>(sql, new { ModId = mod.Id }).ToList();
        }

        return mod;
    }

    public IEnumerable<Mod> GetAllFromGame(Game game)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Mods WHERE GameId = @GameId;";
        var mods = connection.Query<Mod>(sql, new { GameId = game.Id }).ToList();

        foreach (var mod in mods)
        {
            var archivesSql = @"
                SELECT a.* 
                FROM Archives a
                INNER JOIN ArchiveMod am ON a.Id = am.ArchiveId
                WHERE am.ModId = @ModId;
            ";
            mod.Archives = connection.Query<Archive>(archivesSql, new { ModId = mod.Id }).ToList();
        }

        return mods;
    }
}
