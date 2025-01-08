namespace ModManager.Core.Entities;

public class Game
{
    public string Id { get; private set; }
    public string Name { get; set; }
    public string GamePath { get; private set; }
    public string BackupPath { get; private set; }
    public string ModsPath { get; private set; }
    public ModList Mods { get; set; } = [];
    public List<Archive> Archives { get; set; } = [];

    public string BasePath => Path.Combine(GamePath, "ModStation");

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

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Game() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

}
