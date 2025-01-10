using Microsoft.Extensions.DependencyInjection;
using ModManager.Terminal;
using ModStation.Core.Interfaces;
using ModStation.Core.Repositories;
using ModStation.Core.Services;

internal class Program
{
    public static IServiceProvider Services { get; private set; } = null!;

    private static void Main(string[] args)
    {
        var DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)!, "ModStation");
        var connectionString = $"Data Source={Path.Combine(DataDirectory, "ModStation.db")}";

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IContext>((provider) => new Context(connectionString));

        serviceCollection.AddSingleton<IGameRepository, GameRepository>();
        serviceCollection.AddSingleton<IModRepository, ModRepository>();
        serviceCollection.AddSingleton<IArchiveRepository, ArchiveRepository>();
        serviceCollection.AddSingleton<IArchiveModRepository, ArchiveModRepository>();

        serviceCollection.AddSingleton<IGameService, GameService>();
        serviceCollection.AddSingleton<IModService, ModsService>();
        serviceCollection.AddSingleton<IArchiveService, ArchiveService>();
        serviceCollection.AddSingleton<IFileService, FileService>();

        serviceCollection.AddSingleton<MenuService>();

        Services = serviceCollection.BuildServiceProvider();

        var menuService = Services.GetRequiredService<MenuService>();
        
        Console.Clear();
        menuService.Run();
    }
}