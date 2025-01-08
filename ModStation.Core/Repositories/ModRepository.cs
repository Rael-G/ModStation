using System.Data;
using System.Threading.Tasks;
using Dapper;
using ModManager.Core.Entities;
using ModStation.Core.Interfaces;

namespace ModManager.Core.Repositories;

public class ModRepository(string connectionString) : BaseRepository(connectionString), IModRepository
{
    public async Task CreateAsync(Mod mod)
    {
        using var connection = CreateConnection();
        await CreateAsync(mod, connection);
    }

    public async Task UpdateAsync(Mod mod)
    {
        using var connection = CreateConnection();
        await UpdateAsync(mod, connection);
    }

    public async Task DeleteAsync(Mod mod)
    {
        using var connection = CreateConnection();
        await DeleteAsync(mod, connection);
    }

    public async Task CreateAsync(Mod mod, IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = @"
            INSERT INTO Mods (
                Id, Name, ModPath, IsEnable, ""Order"", GameId
            )
            VALUES (
                @Id, @Name, @ModPath, @IsEnable, @Order, @GameId
            )

        ";

        await connection.ExecuteAsync(sql, mod, transaction);
    }

    public async Task UpdateAsync(Mod mod, IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = @"
            UPDATE Mods
            SET Name = @Name, ModPath = @ModPath, ""Order"" = @Order, IsEnable = @IsEnable
            WHERE Id = @Id;
        ";
        await connection.ExecuteAsync(sql, mod, transaction);
    }

    public async Task DeleteAsync(Mod mod, IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = @"
            DELETE FROM Mods WHERE Id = @Id;
        ";
        await connection.ExecuteAsync(sql, new { mod.Id}, transaction);
    }
}
