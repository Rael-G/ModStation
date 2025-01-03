using System.IO.Compression;
using ModManager.Core.Exceptions;
using ModManager.Core.Services;
using SharpCompress.Archives;

namespace ModManager.Core.Entities;

public class Game
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string GamePath { get; set; }

    public string BackupPath { get; set; }

    public string ModsPath { get; set; }

    public ModList Mods { get; set; } = [];

    public List<Archive> Archives { get; set; } = [];

    public Game(string gamePath, string name)
    {
        Id = Guid.NewGuid().ToString();
        GamePath = gamePath;
        Name = name;
        BackupPath = Path.Combine(gamePath, "Backup");
        ModsPath = Path.Combine(gamePath, "Mods");
        Directory.CreateDirectory(BackupPath);
        Directory.CreateDirectory(ModsPath);
    }

    public Game(string id, string name, string gamePath, string backupPath, string modsPath)
    {
        Id = id;
        Name = name;
        GamePath = gamePath;
        BackupPath = backupPath;
        ModsPath = modsPath;
    }

    public Game()
    {
        
    }

    public void InstallMod(string archiveFilePath, string modName)
    {
        if (!File.Exists(archiveFilePath) && !Directory.Exists(archiveFilePath))
        {
            throw new ModManagerException($"File or Directory not found at \"{archiveFilePath}\".");
        }

        var modId = Guid.NewGuid().ToString();
        var modPath = Path.Combine(ModsPath, modId);

        Directory.CreateDirectory(modPath);

        if (!File.Exists(archiveFilePath) && Directory.Exists(archiveFilePath))
        {
            CopyDirectory(archiveFilePath, ModsPath);
        }
        else
        {
            using var archive = ArchiveFactory.Open(archiveFilePath);
            archive.ExtractToDirectory(modPath);
        }

        var mod = new Mod(modId, modName, modPath, this);
        InjectorService.ModsRepository.Create(mod);
        Mods.Add(mod);
        mod.Install();
        mod.Enable();
    }

    public void UninstallMod(Mod mod)
    {
        mod.Uninstall();
        Mods.Remove(mod);
    }

    public void UpdateMod(Mod mod, string filePath)
    {
        UninstallMod(mod);
        InstallMod(filePath, mod.Name);
    }

    public void SwapOrder(Mod mod, int order)
    {
        mod.Disable();
        Mods.AddAtIndex(mod, order);
        mod.Enable();
    }

    public void RemoveGame()
    {
        foreach (var mod in Mods)
        {
            mod.Uninstall();
        }
        Mods.Clear();

        foreach (var archive in Archives)
        {
            InjectorService.ArchivesRepository.Delete(archive);
        }

        Directory.Delete(BackupPath, recursive: true);
        Directory.Delete(ModsPath, recursive: true);
        InjectorService.GamesRepository.Delete(this);
    }

    private static void CopyDirectory(string sourcePath, string targetPath)
    {
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }

}
