using System.Data;
using System.Threading.Tasks;
using Dapper;
using ModManager.Core.Entities;
using ModStation.Core.Extensions;
using ModStation.Core.Interfaces;

namespace ModManager.Core.Repositories;

public class GameRepository(string connectionString) : BaseRepository(connectionString), IGameRepository
{
    public async Task CreateAsync(Game game)
    {
        using var connection = CreateConnection();
        await CreateAsync(game, connection);
    }

    public async Task UpdateAsync(Game game)
    {
        using var connection = CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        await UpdateAsync(game, connection, transaction);
        transaction.Commit();
    }

    public async Task DeleteAsync(Game game)
    {
        using var connection = CreateConnection();
        await DeleteAsync(game, connection);
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Games;";
        var games = (await connection.QueryAsync<Game>(sql)).ToList();

        foreach (var game in games)
        {
            sql = "SELECT * FROM Mods WHERE GameId = @GameId;";
            game.Mods = (await connection.QueryAsync<Mod>(sql, new { GameId = game.Id })).ToModList();

            sql = "SELECT * FROM Archives WHERE GameId = @GameId;";
            game.Archives = (await connection.QueryAsync<Archive>(sql, new { GameId = game.Id })).ToList();

            foreach (var mod in game.Mods)
            {
                sql = "SELECT ModId, ArchiveId FROM ArchiveMod WHERE ModId = @ModId";
                var modArchives = await connection.QueryAsync<(string ModId, string ArchiveId)>(sql, new { ModId = mod.Id });

                mod.Archives = game.Archives
                    .Where(a => modArchives.Any(ma => ma.ModId == mod.Id && ma.ArchiveId == a.Id))
                    .ToList();

                mod.Game = game;
            }

            foreach (var archive in game.Archives)
            {
                sql = "SELECT ModId, ArchiveId FROM ArchiveMod WHERE ArchiveId = @ArchiveId";
                var modArchives = await connection.QueryAsync<(string ModId, string ArchiveId)>(sql, new { ArchiveId = archive.Id });

                archive.Mods = game.Mods
                    .Where(m => modArchives.Any(ma => ma.ArchiveId == archive.Id && ma.ModId == m.Id))
                    .ToList();
                
                archive.Game = game;
            }
        }

        return games;
    }

    public async Task CreateAsync(Game game, IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = @"
            INSERT INTO Games (
                Id, Name, GamePath, BackupPath, ModsPath
            )
            VALUES (
                @Id, @Name, @GamePath, @BackupPath, @ModsPath
            )

        ";

        await connection.ExecuteAsync(sql, game, transaction);
    }

    public async Task UpdateAsync(Game game, IDbConnection connection, IDbTransaction transaction)
    {
        try
        {
            var sql = @"
                UPDATE Games
                SET Name = @Name, GamePath = @GamePath, BackupPath = @BackupPath, ModsPath = @ModsPath
                WHERE Id = @Id;
            ";
            await connection.ExecuteAsync(sql, game, transaction);

            sql = @"
                UPDATE Mods
                SET Name = @Name, ModPath = @ModPath, IsEnable = @IsEnable, ""Order"" = @Order, GameId = @GameId
                WHERE Id = @Id;
            ";

            foreach (var mod in game.Mods)
            {
                await connection.ExecuteAsync(sql, new
                {
                    mod.Id,
                    mod.Name,
                    mod.ModPath,
                    mod.IsEnable,
                    mod.Order,
                    GameId = game.Id
                }, transaction);
            }

            var updateArchiveSql = @"
                UPDATE Archives
                SET RelativePath = @RelativePath, GameId = @GameId
                WHERE Id = @Id;
            ";

            foreach (var archive in game.Archives)
            {
                await connection.ExecuteAsync(updateArchiveSql, new
                {
                    archive.Id,
                    archive.RelativePath,
                    GameId = game.Id
                }, transaction);
            }
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task DeleteAsync(Game game, IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = "DELETE FROM Games WHERE Id = @Id;";
        await connection.ExecuteAsync(sql, game, transaction);
    }
}
