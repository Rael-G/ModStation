using ModManager.Core.Services;

namespace ModManager.Core.Entities;

public class Mod
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string ModPath { get; set; }

    public Game Game { get; set; }

    public bool IsEnable { get; set; }

    public List<Archive> Archives { get; set; }

    public int Order { get; set; }

    public string GameId => Game.Id;

    public Mod(string id, string name, string modPath, Game game, bool isEnable = false, List<Archive>? archives = null)
    {
        Id = id;
        Name = name;
        ModPath = modPath;
        Game = game;
        IsEnable = isEnable;
        Archives = archives?? [];
    }

    public Mod()
    {

    }

    public void Install()
    {
        foreach (var filePath in Directory.GetFiles(ModPath, "*", SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(ModPath, filePath);
            
            var archive = Game.Archives.Where(a => a.RelativePath == relativePath).FirstOrDefault();

            if (archive is null)
            {
                archive = new(Guid.NewGuid().ToString(), relativePath, Game, []);
                InjectorService.ArchivesRepository.Create(archive);
            }
            Archives.Add(archive);
            archive.Mods.Add(this);
            InjectorService.ArchiveModRepository.Create(archive.Id, Id);
        }
    }

    public void Uninstall()
    {
        Disable();

        foreach (var archive in Archives)
        {
            InjectorService.ArchiveModRepository.Delete(archive.Id, Id);
        }
        InjectorService.ModsRepository.Delete(this);
        
        if (Directory.Exists(ModPath))
        {
            Directory.Delete(ModPath, true);
        }
    }

    public void Enable()
    {
        if (IsEnable)
        {
            return;
        }

        foreach (var archive in Archives)
        {
            if (!archive.Mods.Any(m => m.Order > Order && m.IsEnable))
            {
                var targetDirectory = Path.GetDirectoryName(archive.TargetPath);
                if (!string.IsNullOrEmpty(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                File.Copy(archive.ModPath(this), archive.TargetPath, true);
            }
        }

        IsEnable = true;
    }

    public void Disable()
    {
        if (!IsEnable)
        {
            return;
        }

        IsEnable = false;

        foreach (var archive in Archives)
        {
            if (File.Exists(archive.TargetPath)) 
            { 
                File.Delete(archive.TargetPath);
            }

            string fileToReplace;
            var highOrderMod = archive.Mods.Where(m => m.IsEnable).OrderByDescending(m => m.Order).FirstOrDefault();

            if(highOrderMod is not null)
            {
                // Restore the file from the high ordered mod
                fileToReplace = Path.Combine(highOrderMod.ModPath, archive.RelativePath);
            }
            else
            {
                // If there are no other mods overwriting this file, restore the original file
               fileToReplace = archive.BackupPath;
            }

            if (File.Exists(fileToReplace))
            {
                File.Copy(fileToReplace, archive.TargetPath, true);
            }
        }
    }
}
