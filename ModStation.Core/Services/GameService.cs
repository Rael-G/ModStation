using ModManager.Core.Entities;
using ModStation.Core.Interfaces;

namespace ModStation.Core.Services;

public class GameService(IGameRepository gameRepository, IArchiveService archiveService, IModService modService, IFileService fileService)
{
    private readonly IGameRepository _gameRepository = gameRepository;
    private readonly IArchiveService _archiveService = archiveService;
    private readonly IModService _modService = modService;
    private readonly IFileService _fileService = fileService;

    public static string BasePath(string gamePath) => Path.Combine(gamePath, "ModStation");

    public Game Create(string gamePath, string name)
    {
        var id = Guid.NewGuid().ToString();
        var backupPath = Path.Combine(BasePath(gamePath), "Backup");
        var modsPath = Path.Combine(BasePath(gamePath), "Mods");
        _fileService.CreateDirectory(backupPath);
         _fileService.CreateDirectory(modsPath);

        var game = new Game(id, name, gamePath, backupPath, modsPath, [], []);
        try
        {
            _gameRepository.Create(game);
        }
        catch
        {
            _fileService.DeleteDirectory(backupPath);
            _fileService.DeleteDirectory(modsPath);
        }

        return game;
    }

    public void Delete(Game game)
    {
        DeleteAllMods(game);
        RestoreBackup(game);
        ClearArchives(game);
        _fileService.DeleteDirectory(game.GamePath);
        _gameRepository.Delete(game);
    }

    public void Update(Game game)
    {
        _gameRepository.Update(game);
    }

    public void Update(IEnumerable<Game> games)
    {
        foreach (var game in games)
        {
            Update(game);
        }
    }

    private void ClearArchives(Game game)
    {
        foreach (var archive in game.Archives)
        {
            _archiveService.Delete(archive);
        }
        game.Archives.Clear();
    }

    private void RestoreBackup(Game game)
    {
        foreach (var archive in game.Archives)
        {
            _fileService.DeleteFile(archive.TargetPath);
                
            if (!string.IsNullOrEmpty(archive.BackupPath) && File.Exists(archive.BackupPath))
            {
                File.Move(archive.BackupPath, archive.TargetPath, overwrite: true);
            }
        }
    }

    private void DeleteAllMods(Game game)
    {
        foreach (var mod in game.Mods)
        {
            _modService.Delete(mod);
        }
        game.Mods.Clear();
    }
}
