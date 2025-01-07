using System.Data;
using Dapper;
using ModManager.Core.Entities;

namespace ModManager.Core.Repositories;

public class ModRepository(string connectionString) : BaseRepository(connectionString)
{
    public void Create(Mod mod)
    {
        using var connection = CreateConnection();
        Create(mod, connection);
    }

    public void Update(Mod mod)
    {
        using var connection = CreateConnection();
        Update(mod, connection);
    }

    public void Delete(Mod mod)
    {
        using var connection = CreateConnection();
        Delete(mod, connection);
    }

    public void Create(Mod mod, IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = @"
            INSERT INTO Mods (
                Id, Name, ModPath, IsEnable, ""Order"", GameId
            )
            VALUES (
                @Id, @Name, @ModPath, @IsEnable, @Order, @GameId
            )

        ";

        connection.Execute(sql, mod, transaction);
    }

    public void Update(Mod mod, IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = @"
            UPDATE Mods
            SET Name = @Name, ModPath = @ModPath, ""Order"" = @Order, IsEnable = @IsEnable
            WHERE Id = @Id;
        ";
        connection.Execute(sql, mod, transaction);
    }

    public void Delete(Mod mod, IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = @"
            DELETE FROM Mods WHERE Id = @Id;
        ";
        connection.Execute(sql, new { mod.Id}, transaction);
    }
}
