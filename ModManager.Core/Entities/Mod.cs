
namespace ModManager;

public class Mod
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string ModPath { get; set; }

    public Game Game { get; set; }

    public bool IsEnable { get; private set; }

    public List<string> OverwrittenFiles { get; private set; }

    public Mod(Guid id, string name, string modPath, Game game, bool isEnable = false, List<string>? overwrittenFiles = null)
    {
        Id = id;
        Name = name;
        ModPath = modPath;
        Game = game;
        IsEnable = isEnable;
        OverwrittenFiles = overwrittenFiles?? [];
    }

    public Mod()
    {
        
    }

    public void Enable()
    {
        foreach (var filePath in Directory.GetFiles(ModPath, "*", SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(ModPath, filePath);
            string targetPath = Path.Combine(Game.GamePath, relativePath);

            // Original file backup
            if (File.Exists(targetPath))
            {
                string backupPath = Path.Combine(Game.BackupPath, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(backupPath));
                try
                {
                    File.Copy(targetPath, backupPath, false);
                }
                catch(IOException)
                {
                    // if the file already exists, its ok
                }
            }

            OverwrittenFiles.Add(targetPath);

            // Overwrite game files with mod
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
            File.Copy(filePath, targetPath, true);
        }

        IsEnable = true;
    }

    public void Disable()
    {
        if (!IsEnable)
        {
            return;
        }

        foreach (var filePath in OverwrittenFiles)
        {
            string relativePath = Path.GetRelativePath(Game.GamePath, filePath);
            string backupPath = Path.Combine(Game.BackupPath, relativePath);

            File.Delete(filePath);

            // Restore original files
            if (File.Exists(backupPath))
            {
                File.Copy(backupPath, filePath, true);
            }
        }

        IsEnable = false;
    }
}
