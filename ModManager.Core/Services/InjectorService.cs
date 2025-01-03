using System.Reflection;
using ModManager.Core.Repositories;

namespace ModManager.Core.Services;

public class InjectorService
{
    public static string DataDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)!, "ModManager");
    
    private static readonly string _connectionString = $"Data Source={Path.Combine(DataDirectory, "ModManager.db")}";
    
    public static GamesRepository GamesRepository => new(_connectionString);

    public static ModsRepository ModsRepository => new(_connectionString);

    public static ArchivesRepository ArchivesRepository => new(_connectionString);

    public static GamesRepository GameRepository => new(_connectionString);

    public static ArchiveModRepository ArchiveModRepository => new(_connectionString);

}
