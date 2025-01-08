using System.Data;
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

    public async Task<Mod> CreateAsync(string modName, string sourcePath, Game game)
    {
        using var connection = _modRepository.Context.CreateConnection();
        using var transaction = _modRepository.Context.BeginTransaction(connection);
        Mod mod = null!;
        var modPath = string.Empty;
        try
        {
            _fileService.ValidatePath(sourcePath);

            var modId = Guid.NewGuid().ToString();
            modPath = _fileService.CreateDirectory(game.ModsPath, modName);

            mod = new Mod(modId, modName, modPath, game, []);
            await _modRepository.CreateAsync(mod, connection, transaction);

            if (Directory.Exists(sourcePath))
            {
                await _fileService.CopyDirectoryAsync(sourcePath, modPath);
            }
            else
            {
                await _fileService.ExtractArchiveAsync(sourcePath, modPath);
            }

            game.Mods.Add(mod);

            foreach (var filePath in Directory.GetFiles(mod.ModPath, "*", SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(mod.ModPath, filePath);
                var archive = mod.Game.Archives.FirstOrDefault(a => a.RelativePath == relativePath) 
                            ?? await CreateAndRegisterArchiveAsync(mod, relativePath, connection, transaction);

                mod.Archives.Add(archive);
                mod.Game.Archives.Add(archive);
                await _archiveModRepository.CreateAsync(archive.Id, mod.Id, connection, transaction);
                archive.Mods.Add(mod);
            }
        }
        catch
        {
            transaction.Rollback();
            await _fileService.DeleteDirectoryAsync(modPath);
            game.Mods.Remove(mod);
            
            throw;
        }

        transaction.Commit();
        return mod;
    }

    public async Task DeleteAsync(Mod mod)
    {
        if (mod.IsEnable) await DisableAsync(mod);

        await UnlinkArchivesAsync(mod);
        await _fileService.DeleteDirectoryAsync(mod.ModPath);

        mod.Game.Mods.Remove(mod);
    }

    public async Task UpdateAsync(Mod mod)
    {
        await _modRepository.UpdateAsync(mod);
    }

    // public void Update(Game game, Mod mod, string filePath)
    // {
    //     Delete(mod);
    //     Create(mod.Name, filePath, game);
    // }

    public async Task EnableAsync(Mod mod)
    {
        if (mod.IsEnable) return;

        foreach (var archive in mod.Archives)
        {
            if (!mod.IsOverwrittenByLowerOrderMod(archive))
            {
                mod.EnsureTargetDirectoryExists(archive.TargetPath);
                await _fileService.DeleteFileAsync(archive.TargetPath);
                File.CreateSymbolicLink(archive.TargetPath, archive.ModPath(mod));
            }
        }

        mod.IsEnable = true;
        await UpdateAsync(mod);
    }

    public async Task DisableAsync(Mod mod)
    {
        if (!mod.IsEnable) return;

        mod.IsEnable = false;

        foreach (var archive in mod.Archives)
        {
            await _fileService.DeleteFileAsync(archive.TargetPath);

            await _fileService.DeleteFileAsync(archive.TargetPath);

            string replacementFile = mod.GetReplacementFilePath(archive);
            if (!string.IsNullOrEmpty(replacementFile) && File.Exists(replacementFile))
            {
                await Task.Run(() => File.CreateSymbolicLink(archive.TargetPath, replacementFile));
            }
        }

        await UpdateAsync(mod);
    }

    public async Task SwapOrderAsync(Mod mod, int order)
    {
        var enabled = mod.IsEnable;

        await DisableAsync(mod);

        mod.Game.Mods.Insert(order, mod);

        foreach(var m in mod.Game.Mods)
        {
            await UpdateAsync(m);
        }

        if (enabled) await EnableAsync(mod);
    }

    private async Task<Archive> CreateAndRegisterArchiveAsync(Mod mod, string relativePath, IDbConnection connection, IDbTransaction transaction)
    {
        var archive = new Archive(Guid.NewGuid().ToString(), relativePath, mod.Game, []);
        await _archiveRepository.CreateAsync(archive, connection, transaction);
        return archive;
    }

    private async Task UnlinkArchivesAsync(Mod mod)
    {
        foreach (var archive in mod.Archives)
        {
            await _archiveModRepository.DeleteAsync(archive.Id, mod.Id);
        }

        await _modRepository.DeleteAsync(mod);
    }
}
