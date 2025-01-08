namespace ModManager.Core.Entities;

public class Mod : IComparable<Mod>
{
    private int _order;

    public string Id { get; private set; }
    public string Name { get; set; }
    public string ModPath { get; private set; }
    public Game Game { get; set; }
    public List<Archive> Archives { get; set; }
    public bool IsEnabled { get; set; }
    public string GameId { get; private set; }

    public int Order 
    { 
        get => _order; 
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Order)} should not be less than zero.");
            _order = value;
        }
    }

    public Mod(string id, string name, string modPath, Game game, List<Archive> archives, bool isEnable = false)
    {
        Id = id;
        Name = name;
        ModPath = modPath;
        Game = game;
        GameId = game.Id;
        Archives = archives;
        IsEnabled = isEnable;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Mod() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public bool IsOverwrittenByLowerOrderMod(Archive archive)
    {
        return archive.Mods.Any(m => m.Order < Order && m.IsEnabled);
    }

    public void EnsureTargetDirectoryExists(string targetPath)
    {
        var directory = Path.GetDirectoryName(targetPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public string GetReplacementFilePath(Archive archive)
    {
        var lowerOrderMod = archive.Mods
            .Where(m => m.IsEnabled && m.Id != Id)
            .OrderBy(m => m.Order)
            .FirstOrDefault();

        if (lowerOrderMod != null)
        {
            return Path.Combine(lowerOrderMod.ModPath, archive.RelativePath);
        }

        return archive.BackupPath;
    }

    public int CompareTo(Mod? other)
    {
        return Order.CompareTo(other?.Order);
    }
}
