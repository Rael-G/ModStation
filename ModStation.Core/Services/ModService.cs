using ModManager.Core.Entities;
using ModStation.Core.Interfaces;

namespace ModStation.Core.Services;

public class ModsService(IModRepository modRepository, 
    IArchiveRepository archiveRepository, 
    IArchiveModRepository archiveModRepository,
    IFileService fileService)
    : IModService
{
    private readonly IModRepository _modRepository = modRepository;

    private readonly IArchiveRepository _archiveRepository = archiveRepository;

    private readonly IArchiveModRepository _archiveModRepository = archiveModRepository;

    private readonly IFileService _fileService = fileService;

    public Mod Create(string modName, string sourcePath, Game game)
    {
        using var connection = _modRepository.CreateConnection();
        using var transaction = _modRepository.CreateTransaction(connection);
        Mod mod = null!;
        try
        {
            _fileService.ValidatePath(sourcePath);

            var modId = Guid.NewGuid().ToString();
            var modPath = _fileService.CreateDirectory(game.ModsPath, modName);

            mod = new Mod(modId, modName, modPath, game, []);
            _modRepository.Create(mod, connection, transaction);

            if (Directory.Exists(sourcePath))
            {
                _fileService.CopyDirectory(sourcePath, modPath);
            }
            else
            {
                _fileService.ExtractArchive(sourcePath, modPath);
            }

            game.Mods.Add(mod);

            foreach (var filePath in Directory.GetFiles(mod.ModPath, "*", SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(mod.ModPath, filePath);
                var archive = mod.Game.Archives.FirstOrDefault(a => a.RelativePath == relativePath) 
                            ?? CreateAndRegisterArchive(mod, relativePath);

                mod.Archives.Add(archive);
                mod.Game.Archives.Add(archive);
                LinkArchiveToMod(mod, archive);
            }
        }
        catch
        {
            transaction.Rollback();
        }

        transaction.Commit();
        return mod;
    }

    public void Delete(Mod mod)
    {
        if (mod.IsEnable) Disable(mod);

        UnlinkArchives(mod);
        DeleteModFiles(mod);

        mod.Game.Mods.Remove(mod);
    }

    // public void Update(Game game, Mod mod, string filePath)
    // {
    //     Delete(mod);
    //     Create(mod.Name, filePath, game);
    // }

    public void Enable(Mod mod)
    {
        if (mod.IsEnable) return;

        foreach (var archive in mod.Archives)
        {
            if (!mod.IsOverwrittenByLowerOrderMod(archive))
            {
                mod.EnsureTargetDirectoryExists(archive.TargetPath);
                _fileService.DeleteFile(archive.TargetPath);
                File.CreateSymbolicLink(archive.TargetPath, archive.ModPath(mod));
            }
        }

        mod.IsEnable = true;
    }

    public void Disable(Mod mod)
    {
        if (!mod.IsEnable) return;

        mod.IsEnable = false;

        foreach (var archive in mod.Archives)
        {
            _fileService.DeleteFile(archive.TargetPath);

            _fileService.DeleteFile(archive.TargetPath);

            string replacementFile = mod.GetReplacementFilePath(archive);
            if (!string.IsNullOrEmpty(replacementFile) && File.Exists(replacementFile))
            {
                File.CreateSymbolicLink(archive.TargetPath, replacementFile);
            }
        }
    }

    public void SwapOrder(Mod mod, int order)
    {
        var enabled = mod.IsEnable;

        if (enabled) Disable(mod);

        mod.Game.Mods.Insert(order, mod);
        
        if (enabled) Enable(mod);
    }

    private Archive CreateAndRegisterArchive(Mod mod, string relativePath)
    {
        var archive = new Archive(Guid.NewGuid().ToString(), relativePath, mod.Game, []);
        _archiveRepository.Create(archive);
        return archive;
    }

    private void LinkArchiveToMod(Mod mod, Archive archive)
    {
        archive.Mods.Add(mod);
        _archiveModRepository.Create(archive.Id, mod.Id);
    }

    private void UnlinkArchives(Mod mod)
    {
        foreach (var archive in mod.Archives)
        {
            _archiveModRepository.Delete(archive.Id, mod.Id);
        }

        _modRepository.Delete(mod);
    }

    private void DeleteModFiles(Mod mod)
    {
        if (Directory.Exists(mod.ModPath))
        {
            Directory.Delete(mod.ModPath, recursive: true);
        }
    }
}
