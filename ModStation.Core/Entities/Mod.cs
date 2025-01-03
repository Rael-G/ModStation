using ModManager.Core.Services;

namespace ModManager.Core.Entities;

public class Mod
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string ModPath { get; private set; }
    public Game Game { get; set; }
    public List<Archive> Archives { get; set; }
    public bool IsEnable { get; private set; }
    public int Order { get; set; }
    public string GameId { get; private set; }

    public Mod(string id, string name, string modPath, Game game, List<Archive> archives, bool isEnable = false)
    {
        Id = id;
        Name = name;
        ModPath = modPath;
        Game = game;
        GameId = game.Id;
        Archives = archives;
        IsEnable = isEnable;
    }

    public Mod() { }

    public void Install()
    {
        foreach (var filePath in Directory.GetFiles(ModPath, "*", SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(ModPath, filePath);
            var archive = Game.Archives.FirstOrDefault(a => a.RelativePath == relativePath) 
                          ?? CreateAndRegisterArchive(relativePath);

            Archives.Add(archive);
            Game.Archives.Add(archive);
            LinkArchiveToMod(archive);
        }
    }

    public void Uninstall()
    {
        if (IsEnable) Disable();

        UnlinkArchives();
        DeleteModFiles();
    }

    public void Enable()
    {
        if (IsEnable) return;

        foreach (var archive in Archives)
        {
            if (!IsOverwrittenByLowerOrderMod(archive))
            {
                EnsureTargetDirectoryExists(archive.TargetPath);
                DeleteFileIfExists(archive.TargetPath);
                File.CreateSymbolicLink(archive.TargetPath, archive.ModPath(this));
            }
        }

        IsEnable = true;
    }

    public void Disable()
    {
        if (!IsEnable) return;

        IsEnable = false;

        foreach (var archive in Archives)
        {
            DeleteFileIfExists(archive.TargetPath);

            DeleteFileIfExists(archive.TargetPath);

            string replacementFile = GetReplacementFilePath(archive);
            if (!string.IsNullOrEmpty(replacementFile) && File.Exists(replacementFile))
            {
                File.CreateSymbolicLink(archive.TargetPath, replacementFile);
            }
        }
    }

    private Archive CreateAndRegisterArchive(string relativePath)
    {
        var archive = new Archive(Guid.NewGuid().ToString(), relativePath, Game, new List<Mod>());
        InjectorService.ArchivesRepository.Create(archive);
        return archive;
    }

    private void LinkArchiveToMod(Archive archive)
    {
        archive.Mods.Add(this);
        InjectorService.ArchiveModRepository.Create(archive.Id, Id);
    }

    private void UnlinkArchives()
    {
        foreach (var archive in Archives)
        {
            InjectorService.ArchiveModRepository.Delete(archive.Id, Id);
        }

        InjectorService.ModsRepository.Delete(this);
    }

    private void DeleteModFiles()
    {
        if (Directory.Exists(ModPath))
        {
            Directory.Delete(ModPath, recursive: true);
        }
    }

    private bool IsOverwrittenByLowerOrderMod(Archive archive)
    {
        return archive.Mods.Any(m => m.Order < Order && m.IsEnable);
    }

    private void EnsureTargetDirectoryExists(string targetPath)
    {
        var directory = Path.GetDirectoryName(targetPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    private string GetReplacementFilePath(Archive archive)
    {
        var lowerOrderMod = archive.Mods
            .Where(m => m.IsEnable && m.Id != Id)
            .OrderBy(m => m.Order)
            .FirstOrDefault();

        if (lowerOrderMod != null)
        {
            return Path.Combine(lowerOrderMod.ModPath, archive.RelativePath);
        }

        return archive.BackupPath;
    }

    private void DeleteFileIfExists(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
