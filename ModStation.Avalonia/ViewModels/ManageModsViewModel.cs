using System.Collections.ObjectModel;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModManager;
using ModManager.Core.Entities;
using ModManager.Core.Exceptions;

namespace ModStation.Avalonia.ViewModels;

public partial class ManageModsViewModel(Game game, Manager manager) : ViewModelBase
{
    private readonly Manager _manager = manager;
    private readonly Game _game = game;

    public string GameName => game.Name;

    public ObservableCollection<Mod> Mods { get; set; } = new(game.Mods);

    [ObservableProperty]
    private Mod? _selectedMod;

    [RelayCommand]
    public async Task InstallMod()
    {
        var mainWindow = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

        if (mainWindow == null)
            return;

        var selectedFile = await mainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Mod Path",
            AllowMultiple = false
        });

        if (selectedFile != null && selectedFile.Count > 0)
        {
            var modPath = selectedFile[0].Path.LocalPath;
            var modName = Path.GetFileNameWithoutExtension(modPath);

            try
            {
                var mod = game.InstallMod(modPath, modName);
                Mods.Add(mod);
            }
            catch (DuplicatedEntityException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    [RelayCommand]
    public void ChangeOrder()
    {
        // Open a dialog or navigate to an order management page
    }

    [RelayCommand]
    public void ToggleMod(Mod mod)
    {
        if (mod.IsEnable)
        {
            mod.Disable();
        }
        else
        {
            mod.Enable();
        }

        var index = Mods.IndexOf(mod);
        if (index >= 0)
        {
            Mods[index] = mod;
        }
    }

    [RelayCommand]
    public void UninstallMod(Mod mod)
    {
        mod.Game.UninstallMod(mod);
        Mods.Remove(mod);
    }

    
}
