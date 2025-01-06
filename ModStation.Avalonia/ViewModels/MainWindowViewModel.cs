using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using ModManager;

namespace ModStation.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentView = null!;

    public MainWindowViewModel(Manager manager)
    {
        OpenGamesView();
    }

    public void OpenGamesView()
    {
        CurrentView = App.Services.GetRequiredService<ManageGamesViewModel>();
    }
}
