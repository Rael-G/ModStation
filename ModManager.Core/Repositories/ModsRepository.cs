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
}
