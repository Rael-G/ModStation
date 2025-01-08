using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using ModStation.Avalonia.ViewModels;
using ModStation.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using ModStation.Core.Interfaces;
using ModManager.Core.Repositories;
using ModStation.Core.Services;

namespace ModStation.Avalonia;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;
    
    public static MainWindow MainWindow { get; set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped<IGameRepository, GameRepository>();
            serviceCollection.AddScoped<IModRepository, ModRepository>();
            serviceCollection.AddScoped<IArchiveRepository, ArchiveRepository>();
            serviceCollection.AddScoped<IArchiveModRepository, IArchiveModRepository>();

            serviceCollection.AddScoped<IGameService, GameService>();
            serviceCollection.AddScoped<IModService, ModsService>();
            serviceCollection.AddScoped<IArchiveService, ArchiveService>();
            serviceCollection.AddScoped<IFileService, FileService>();

            serviceCollection.AddSingleton<MainWindowViewModel>();
            serviceCollection.AddSingleton<ManageGamesViewModel>();

            Services = serviceCollection.BuildServiceProvider();

            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainWindowViewModel>(),
            };
            desktop.MainWindow = MainWindow;

        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}