using System.Collections.ObjectModel;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModManager;

namespace ModStation.Avalonia.ViewModels;

public partial class MainWindowViewModel(Manager manager) : ObservableObject
{
    private readonly Manager _manager = manager;

    private ObservableCollection<string> _gameList = [.. manager.Games.Select(g => g.Name)];

    public ObservableCollection<string> GameList
    {
        get => _gameList;
        set => SetProperty(ref _gameList, value);
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
                GameList.Add(gamePath);
            }
        }
    }
}
