namespace ModManager.Core.Entities;

public class Archive
{
    public string Id { get; private set; }
    public string RelativePath { get; private set; }
    public Game Game { get; set; }
    public List<Mod> Mods { get; set; }
    public string GameId { get; private set; }

    public string TargetPath => Path.Combine(Game.GamePath, RelativePath);

    public string BackupPath => Path.Combine(Game.BackupPath, RelativePath);

    public string ModPath(Mod mod) => Path.Combine(mod.ModPath, RelativePath);

    public Archive(string id, string relativePath, Game game, List<Mod> mods)
    {
        Id = id;
        RelativePath = relativePath;
        Game = game;
        GameId = game.Id;
        Mods = mods;
        Backup();
    }

    public Archive() { }

    private void Backup()
    {
        if (File.Exists(TargetPath) && !File.Exists(BackupPath))
        {
            CreateDirectoryIfNotExists(Path.GetDirectoryName(BackupPath));
            File.Copy(TargetPath, BackupPath, overwrite: false);
        }
    }

    private static void CreateDirectoryIfNotExists(string? directoryPath)
    {
        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }
}
