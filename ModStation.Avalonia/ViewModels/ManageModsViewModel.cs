using System.Collections.ObjectModel;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModManager;
using ModManager.Core.Entities;
using ModManager.Core.Exceptions;
using ModStation.Avalonia.Extensions;
using ModStation.Avalonia.Views;

namespace ModStation.Avalonia.ViewModels;

public partial class ManageModsViewModel(Game game, Manager manager) : ViewModelBase
{
    private readonly Game _game = game;
    private readonly Manager _manager = manager;

    public string GameName => _game.Name;

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
                var mod = _game.InstallMod(modPath, modName);
                Mods.Add(mod);
            }
            catch (DuplicatedEntityException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        _manager.Save();
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

        Mods.Refresh(mod);
        _manager.Save();
    }

    [RelayCommand]
    public void UninstallMod(Mod mod)
    {
        mod.Game.UninstallMod(mod);
        Mods.Remove(mod);
        _manager.Save();
    }

    [RelayCommand]
    private void MoveUp(Mod mod)
    {
        var index = Mods.IndexOf(mod);
        if (index > 0)
        {
            var temp = Mods[index - 1];
            Mods[index - 1] = mod;
            Mods[index] = temp;
        }
        _game.SwapOrder(mod, index - 1);
        Mods.Refresh(mod);
        _manager.Save();
    }

    [RelayCommand]
    private void MoveDown(Mod mod)
    {
        var index = Mods.IndexOf(mod);
        if (index < Mods.Count - 1)
        {
            var temp = Mods[index + 1];
            Mods[index + 1] = mod;
            Mods[index] = temp;
        }
        _game.SwapOrder(mod, index + 1);
        Mods.Refresh(mod);
        _manager.Save();
    }
    
}
