using System.Collections.ObjectModel;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using ModManager;
using System.Linq;
using ModManager.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using ModStation.Avalonia.Views;
using ModManager.Core.Exceptions;
using ModStation.Avalonia.Extensions;

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
            var mainWindow = App.MainWindow;

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
                    var folderName = Path.GetFileName(gamePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                    var gameNameDialog = new EnterNameDialog()
                    {
                        NameText = folderName
                    };

                    var result = await gameNameDialog.ShowDialog<bool>(mainWindow);

                    if (result && !string.IsNullOrEmpty(gameNameDialog.Name))
                    {
                        try
                        {
                            var game = _manager.AddGame(gamePath, gameNameDialog.Name);
                            Games.Add(game);
                        }
                        catch (DuplicatedEntityException e)
                        {
                            // Exibe mensagem de erro (pode ser um Toast ou MessageBox no futuro)
                            Console.WriteLine($"Duplicated game: {e.Message}");
                        }
                    }
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

        [RelayCommand]
        public async Task RenameGame(Game game)
        {
            var gameNameDialog = new EnterNameDialog()
                {
                    NameText = game.Name
                };

            var result = await gameNameDialog.ShowDialog<bool>(App.MainWindow);
            if (result)
            {
                game.Name = gameNameDialog.NameText;   
                _manager.Save();
                Games.Refresh(game);
            }
        }
    }
}
