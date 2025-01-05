using System.Data.Common;
using System.IO.Compression;
using ModManager.Core.Exceptions;
using ModManager.Core.Services;
using SharpCompress.Archives;

namespace ModManager.Core.Entities;

public class Game
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string GamePath { get; private set; }
    public string BackupPath { get; private set; }
    public string ModsPath { get; private set; }
    public ModList Mods { get; set; } = [];
    public List<Archive> Archives { get; set; } = [];

    // Primary constructor
    public Game(string gamePath, string name)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        GamePath = gamePath;
        InitializePaths();
    }

    // Overloaded constructor for existing games
    public Game(string id, string name, string gamePath, string backupPath, string modsPath, ModList mods, List<Archive> archives)
    {
        Id = id;
        Name = name;
        GamePath = gamePath;
        BackupPath = backupPath;
        ModsPath = modsPath;
        Mods = mods;
        Archives = archives;
    }

    // Parameterless constructor for serialization
    public Game() { }

    private void InitializePaths()
    {
        BackupPath = Path.Combine(GamePath, "ModStation", "Backup");
        ModsPath = Path.Combine(GamePath, "ModStation", "Mods");
        Directory.CreateDirectory(BackupPath);
        Directory.CreateDirectory(ModsPath);
    }

    public Mod InstallMod(string archiveFilePath, string modName)
    {
        ValidatePath(archiveFilePath);

        var modId = Guid.NewGuid().ToString();
        var modPath = Path.Combine(ModsPath, modId);
        Directory.CreateDirectory(modPath);

        if (Directory.Exists(archiveFilePath))
        {
            CopyDirectory(archiveFilePath, modPath);
        }
        else
        {
            ExtractArchiveToDirectory(archiveFilePath, modPath);
        }

        var mod = new Mod(modId, modName, modPath, this, []);
        try
        {
            InjectorService.ModsRepository.Create(mod);
        }
        catch(DbException e)
        {
            Directory.Delete(modPath, true);
            throw new DuplicatedEntityException("There alredy is a mod with this name.", e);
        }

        Mods.Add(mod);
        mod.Install();
        
        return mod;
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
        UninstallAllMods();
        RestoreBackup();
        ClearArchives();
        DeleteDirectories();
        InjectorService.GamesRepository.Delete(this);
    }

    private void RestoreBackup()
    {
        foreach (var archive in Archives)
        {
            if (File.Exists(archive.TargetPath))
            {
                File.Delete(archive.TargetPath);
            } 
                
            if (!string.IsNullOrEmpty(archive.BackupPath) && File.Exists(archive.BackupPath))
            {
                File.Copy(archive.BackupPath, archive.TargetPath, overwrite: true);
            }
        }
    }

    private void UninstallAllMods()
    {
        foreach (var mod in Mods)
        {
            mod.Uninstall();
        }
        Mods.Clear();
    }

    private void ClearArchives()
    {
        foreach (var archive in Archives)
        {
            InjectorService.ArchivesRepository.Delete(archive);
        }
        Archives.Clear();
    }

    private void DeleteDirectories()
    {
        if (Directory.Exists(BackupPath)) Directory.Delete(BackupPath, recursive: true);
        if (Directory.Exists(ModsPath)) Directory.Delete(ModsPath, recursive: true);
    }

    private static void ValidatePath(string path)
    {
        if (!File.Exists(path) && !Directory.Exists(path))
        {
            throw new ModManagerException($"File or Directory not found at \"{path}\".");
        }
    }

    private static void CopyDirectory(string sourcePath, string targetPath)
    {
        foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(Path.Combine(targetPath, Path.GetRelativePath(sourcePath, dirPath)));
        }

        foreach (var filePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            var targetFilePath = Path.Combine(targetPath, Path.GetRelativePath(sourcePath, filePath));
            File.Copy(filePath, targetFilePath, overwrite: true);
        }
    }

    private static void ExtractArchiveToDirectory(string archiveFilePath, string targetPath)
    {
        using var archive = ArchiveFactory.Open(archiveFilePath);
        archive.ExtractToDirectory(targetPath);
    }
}
