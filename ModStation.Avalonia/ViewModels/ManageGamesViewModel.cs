using System.Collections.ObjectModel;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using ModManager;
using System.Linq;
using ModManager.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ModStation.Avalonia.ViewModels
{
    public partial class ManageGamesViewModel : ViewModelBase
    {
        private readonly Manager _manager;
        
        public ObservableCollection<Game> Games { get; set; }

        [ObservableProperty]
        private Game? _selectedGame;

        public ManageGamesViewModel(Manager manager)
        {
            _manager = manager;
            Games = new(_manager.Games);
        }

        [RelayCommand]
        public async Task AddGame()
        {
            var mainWindow = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.MainWindow
                : null;

            if (mainWindow == null)
                return;

            var selectedFolders = await mainWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Game Path",
                AllowMultiple = false
            });

            if (selectedFolders != null && selectedFolders.Count > 0)
            {
                var gamePath = selectedFolders[0].Path.LocalPath;
                if (!string.IsNullOrEmpty(gamePath))
                {
                    _manager.AddGame(gamePath, gamePath);
                    Games = new(_manager.Games);
                }
            }
        }

        [RelayCommand]
        public void OpenManageMods(Game game)
        {
            if (game != null)
            {
                App.Services.GetRequiredService<MainWindowViewModel>().CurrentView = new ManageModsViewModel(game, _manager);
            }
        }

        [RelayCommand]
        public void RemoveGame(Game game)
        {
            _manager.RemoveGame(game);
            _manager.Save();
            Games.Remove(game);
        }
    }
}
