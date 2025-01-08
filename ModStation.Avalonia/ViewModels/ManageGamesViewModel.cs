using System.Collections.ObjectModel;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using ModManager.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using ModStation.Avalonia.Views;
using ModStation.Avalonia.Extensions;
using ModStation.Core.Interfaces;
using System.Threading.Tasks;

namespace ModStation.Avalonia.ViewModels
{
    public partial class ManageGamesViewModel : ViewModelBase
    {
        public ObservableCollection<Game> Games { get; set; }

        [ObservableProperty]
        private Game? _selectedGame;

        private readonly IGameService _gameService;
        private readonly IModService _modService;

        public ManageGamesViewModel(IGameService gameService, IModService modService)
        {
            _gameService = gameService;
            _modService = modService;
            Games = [];
            _ = InitializeGamesAsync();
        }

        private async Task InitializeGamesAsync()
        {
            Games = [.. await _gameService.GetAllAsync()];
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
                        NameText = folderName,
                        Description = "Enter the game name:"
                    };

                    var result = await gameNameDialog.ShowDialog<bool>(mainWindow);

                    if (result && !string.IsNullOrEmpty(gameNameDialog.NameText))
                    {
                        try
                        {
                            var game = await _gameService.CreateAsync(gamePath, gameNameDialog.NameText);
                            Games.Add(game);
                        }
                        catch (Exception e)
                        {
                            await new ErrorDialog(){ SecondDescription = e.Message }.ShowDialog<bool>(App.MainWindow);
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
                App.Services.GetRequiredService<MainWindowViewModel>().CurrentView = new ManageModsViewModel(game, _modService);
            }
        }

        [RelayCommand]
        public async Task RemoveGame(Game game)
        {
            var progressDialog = new ProgressDialog()
            {
                Text = $"Removing {game.Name}",
                Description = "Warning: Closing the program now may result in game data corruption."
            };
            progressDialog.Show(App.MainWindow);

            await _gameService.DeleteAsync(game);
            Games.Remove(game);

            progressDialog.Close();
        }

        [RelayCommand]
        public async Task RenameGame(Game game)
        {
            var gameNameDialog = new EnterNameDialog()
                {
                    NameText = game.Name,
                    Description = "Enter the game name:"
                };

            var result = await gameNameDialog.ShowDialog<bool>(App.MainWindow);
            if (result)
            {
                try
                {
                    game.Name = gameNameDialog.NameText;   
                    await _gameService.UpdateAsync(game);
                    Games.Refresh(game);
                }
                catch (Exception e)
                {
                    await new ErrorDialog(){ SecondDescription = e.Message }.ShowDialog<bool>(App.MainWindow);
                }
            }
        }
    }
}
