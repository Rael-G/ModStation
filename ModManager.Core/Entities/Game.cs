using System.IO.Compression;

namespace ModManager;

public class Game
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string GamePath { get; set; }

    public string BackupPath { get; set; }

    public string ModsPath { get; set; }

    public List<Mod> Mods { get; set; } = [];

    public Game(string gamePath)
    {
        Id = Guid.NewGuid();
        GamePath = gamePath;
        Name = Path.GetFileName(gamePath);
        BackupPath = Path.Combine(gamePath, "Backup");
        ModsPath = Path.Combine(gamePath, "Mods");
        Directory.CreateDirectory(BackupPath);
        Directory.CreateDirectory(ModsPath);
    }

    public Game(Guid id, string name, string gamePath, string backupPath, string modsPath)
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

    public void InstallMod(string zipFilePath)
    {
        if (!File.Exists(zipFilePath))
        {
            Console.WriteLine("Zip file not found.");
            return;
        }

        if (Path.GetExtension(zipFilePath) != ".zip")
        {

            Console.WriteLine("This is not a zip file." );
            return;
        }

        var modName = Path.GetFileNameWithoutExtension(zipFilePath);
        var modPath = Path.Combine(ModsPath, modName);

        Directory.CreateDirectory(modPath);
        ZipFile.ExtractToDirectory(zipFilePath, modPath);
        

        var mod = new Mod(Guid.NewGuid(), modName, modPath, this);
        Mods.Add(mod);
        mod.Enable();
    }

    public void UninstallMod(Mod mod)
    {
        mod.Disable();
        if (Directory.Exists(mod.ModPath))
        {
            Directory.Delete(mod.ModPath, true);
        }
        Mods.Remove(mod);
    }

    public void UpdateMod(Mod mod, string zipFilePath)
    {
        UninstallMod(mod);
        InstallMod(zipFilePath);
    }
}
