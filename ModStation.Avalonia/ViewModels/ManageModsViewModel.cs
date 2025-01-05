using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
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
                // Handle error (e.g., show a message box)
            }
        }
    }

    [RelayCommand]
    public void ChangeOrder()
    {
        // Open a dialog or navigate to an order management page
    }

    [RelayCommand]
    public void Back()
    {
        App.Services.GetRequiredService<MainWindowViewModel>().WindowPop();
    }
}

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? Brushes.Green : Brushes.Red;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class EnableStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "(Enabled)" : "(Disabled)";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
