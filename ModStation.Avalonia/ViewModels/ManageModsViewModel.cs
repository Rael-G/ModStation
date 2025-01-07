using System.Collections.ObjectModel;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
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

                progressDialog.Show(mainWindow);
                try
                {
                    await Task.Run( () => 
                    {
                        var mod = _game.InstallMod(modPath, modName);
                        Mods.Insert(0, mod);
                    });
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

        _manager.Save();
    }

    [RelayCommand]
    public void ToggleMod(Mod mod)
    {
        var progressDialog = new ProgressDialog();
        var message = mod.IsEnable ? "Enabling" : "Disabling";
        progressDialog.Text = $"{message} {mod.Name}";

        progressDialog.Show(App.MainWindow);

        Task.Run(() =>
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
        });

        progressDialog.Close();
    }

    [RelayCommand]
    public void UninstallMod(Mod mod)
    {
        var progressDialog = new ProgressDialog()
        {
            Text = $"Unninstaling {mod.Name}",
        };
        progressDialog.Show(App.MainWindow);


        Task.Run(() =>
        {
            mod.Game.UninstallMod(mod);
            Mods.Remove(mod);
            _manager.Save();
        });
        
        progressDialog.Close();
    }

    [RelayCommand]
    private void MoveUp(Mod mod)
    {
        var progressDialog = new ProgressDialog()
        {
            Text = $"Moving up {mod.Name}",
        };
        progressDialog.Show(App.MainWindow);
        
        Task.Run(() =>
        {
            var index = Mods.IndexOf(mod);
            if (index > 0)
            {
                var temp = Mods[index - 1];
                Mods[index - 1] = mod;
                Mods[index] = temp;
                _game.SwapOrder(mod, index - 1);
                Mods.Refresh(mod);
                _manager.Save();
            }
        });
        
        progressDialog.Close();
    }

    [RelayCommand]
    private void MoveDown(Mod mod)
    {
        var progressDialog = new ProgressDialog()
        {
            Text = $"Moving up {mod.Name}",
        };
        progressDialog.Show(App.MainWindow);
        
        Task.Run(() =>
        {
            var index = Mods.IndexOf(mod);
            if (index < Mods.Count - 1)
            {
                var temp = Mods[index + 1];
                Mods[index + 1] = mod;
                Mods[index] = temp;
                _game.SwapOrder(mod, index + 1);
                Mods.Refresh(mod);
                _manager.Save();
            }
        });
        
        progressDialog.Close();
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
                _manager.Save();
                Mods.Refresh(mod); 
            }
            catch (Exception e)
            {
                await new ErrorDialog(){ SecondDescription = e.Message }.ShowDialog<bool>(App.MainWindow);
            }
            
        }
    }
    
}
