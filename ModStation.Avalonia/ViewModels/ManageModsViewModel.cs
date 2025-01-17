using System.Collections.ObjectModel;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ModManager.Core.Entities;
using ModStation.Avalonia.Extensions;
using ModStation.Avalonia.Views;
using ModStation.Core.Interfaces;

namespace ModStation.Avalonia.ViewModels;

public partial class ManageModsViewModel(Game game, IModService modService) : ViewModelBase
{
    private readonly Game _game = game;
    private readonly IModService _modService = modService;

    public string GameName => _game.Name;

    public ObservableCollection<Mod> Mods { get; set; } = new(game.Mods);

    [ObservableProperty]
    private Mod? _selectedMod;

    [RelayCommand]
    public void OpenGamesView()
    {
        App.Services.GetRequiredService<MainWindowViewModel>().OpenGamesView();
    }

    [RelayCommand]
    public async Task InstallMod()
    {
        var mainWindow = App.MainWindow;

        if (mainWindow == null)
            return;

        var selectedFolder = await mainWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Mod Path",
                AllowMultiple = false
            });

        if (selectedFolder != null && selectedFolder.Count > 0)
        {
            var modPath = selectedFolder[0].Path.LocalPath;
            var modName = Path.GetFileName(modPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

            var modNameDialog = new EnterNameDialog()
            {
                NameText = modName,
                Description = "Enter the mod name:"
            };

            var result = await modNameDialog.ShowDialog<bool>(mainWindow);
            modName = modNameDialog.NameText;

            if (result && !string.IsNullOrEmpty(modName))
            {
                var progressDialog = new ProgressDialog()
                {
                    Text = $"Installing {modName}",
                };

                var _ = progressDialog.ShowDialog(mainWindow);
                try
                {
                    var mod = await _modService.CreateAsync(modName, modPath, _game);
                    Mods.Insert(0, mod);
                    Mods.RefreshAll();
                }
                catch (Exception e)
                {
                    await new ErrorDialog(){ SecondDescription = e.Message }.ShowDialog<bool>(App.MainWindow);
                }
                finally
                {
                    progressDialog.Close();
                }
            }
        }
    }

    [RelayCommand]
    public async Task ToggleModAsync(Mod mod)
    {
        if (mod.IsEnabled)
        {
            await _modService.DisableAsync(mod);
        }
        else
        {
            await _modService.EnableAsync(mod);
        }
        Mods.Refresh(mod);
    }

    [RelayCommand]
    public async Task UninstallModAsync(Mod mod)
    {
        var progressDialog = new ProgressDialog()
        {
            Text = $"Unninstaling {mod.Name}",
        };
        progressDialog.Show(App.MainWindow);


        await _modService.DeleteAsync(mod);
        Mods.Remove(mod);
        
        progressDialog.Close();
    }

    [RelayCommand]
    private async Task MoveUpAsync(Mod mod)
    {
        var index = Mods.IndexOf(mod);
        if (index > 0)
        {
            var temp = Mods[index - 1];
            Mods[index - 1] = mod;
            Mods[index] = temp;
            await _modService.SwapOrderAsync(mod, index - 1);
            Mods.Refresh(Mods[index - 1]);
            Mods.Refresh(Mods[index]);
        }
    }

    [RelayCommand]
    private async Task MoveDown(Mod mod)
    {
        var index = Mods.IndexOf(mod);
        if (index < Mods.Count - 1)
        {
            var temp = Mods[index + 1];
            Mods[index + 1] = mod;
            Mods[index] = temp;
            await _modService.SwapOrderAsync(mod, index + 1);
            Mods.Refresh(Mods[index + 1]);
            Mods.Refresh(Mods[index]);
        }
    }

    [RelayCommand]
    public async Task RenameMod(Mod mod)
    {
        var modNameDialog = new EnterNameDialog()
        {
            NameText = mod.Name,
            Description = "Enter the mod name:"
        };

        var result = await modNameDialog.ShowDialog<bool>(App.MainWindow);
        if (result)
        {
            try
            {
                mod.Name = modNameDialog.NameText;   
                await _modService.UpdateAsync(mod);
                Mods.Refresh(mod); 
            }
            catch (Exception e)
            {
                await new ErrorDialog(){ SecondDescription = e.Message }.ShowDialog<bool>(App.MainWindow);
            }
        }
    }
}
