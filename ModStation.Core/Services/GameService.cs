using ModManager.Core.Entities;
using ModStation.Core.Interfaces;

namespace ModStation.Core.Services;

public class GameService(IGameRepository gameRepository, IArchiveService archiveService, IModService modService, IFileService fileService)
    : IGameService
{
    private readonly IGameRepository _gameRepository = gameRepository;
    private readonly IArchiveService _archiveService = archiveService;
    private readonly IModService _modService = modService;
    private readonly IFileService _fileService = fileService;

    public static string BasePath(string gamePath) => Path.Combine(gamePath, "ModStation");

    public async Task<Game> CreateAsync(string gamePath, string name)
    {
        var id = Guid.NewGuid().ToString();
        var backupPath = Path.Combine(BasePath(gamePath), "Backup");
        var modsPath = Path.Combine(BasePath(gamePath), "Mods");
        _fileService.CreateDirectory(backupPath);
        _fileService.CreateDirectory(modsPath);

        var game = new Game(id, name, gamePath, backupPath, modsPath, [], []);
        try
        {
            await _gameRepository.CreateAsync(game);
        }
        catch
        {
            await _fileService.DeleteDirectoryAsync(backupPath);
            await _fileService.DeleteDirectoryAsync(modsPath);
            throw;
        }

        return game;
    }

    public async Task DeleteAsync(Game game)
    {
        await DeleteAllModsAsync(game);
        await RestoreBackupAsync(game);
        await ClearArchivesAsync(game);
        await _fileService.DeleteDirectoryAsync(game.GamePath);
        await _gameRepository.DeleteAsync(game);
    }

    public async Task UpdateAsync(Game game)
    {
        await _gameRepository.UpdateAsync(game);
    }

    public async Task UpdateAsync(IEnumerable<Game> games)
    {
        foreach (var game in games)
        {
            await UpdateAsync(game);
        }
    }

    private async Task ClearArchivesAsync(Game game)
    {
        foreach (var archive in game.Archives)
        {
            await _archiveService.DeleteAsync(archive);
        }
        game.Archives.Clear();
    }

    private async Task RestoreBackupAsync(Game game)
    {
        foreach (var archive in game.Archives)
        {
            await _fileService.DeleteFileAsync(archive.TargetPath);
                
            if (!string.IsNullOrEmpty(archive.BackupPath) && File.Exists(archive.BackupPath))
            {
                await Task.Run(() => File.Move(archive.BackupPath, archive.TargetPath, overwrite: true));
            }
        }
    }

    private async Task DeleteAllModsAsync(Game game)
    {
        foreach (var mod in game.Mods)
        {
            await _modService.DeleteAsync(mod);
        }
        game.Mods.Clear();
    }
}
