namespace ModManager.Core.Entities;

public class Archive
{
    public string Id { get; set; }

    public string RelativePath { get; set; }

    public Game Game { get; set; }

    public List<Mod> Mods { get; set; }

    public string GameId => Game.Id;

    public string TargetPath => Path.Combine(Game.GamePath, RelativePath);
    public string BackupPath => Path.Combine(Game.BackupPath, RelativePath);
    public string ModPath(Mod mod) => Path.Combine(mod.ModPath, RelativePath);

    public Archive(string id, string relativePath, Game game, List<Mod>? mods = null)
    {
        Id = id;
        RelativePath = relativePath;
        Game = game;
        Mods = mods?? [];
        Backup();
    }

    public Archive()
    {
        
    }

    private void Backup()
    {
        if (File.Exists(TargetPath) && !File.Exists(BackupPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(BackupPath));
            File.Copy(TargetPath, BackupPath!, false);
        }
    }

}
