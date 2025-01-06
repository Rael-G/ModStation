using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ModManager.Core.Entities;
using ModStation.Avalonia.ViewModels;

namespace ModStation.Avalonia.Views;

public partial class ManageGamesView : UserControl
{
    public ManageGamesView()
    {
        InitializeComponent();
    }

    private void ListBoxItem_MouseDoubleClick(object sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            var grid = sender as Grid;
            var game = grid?.DataContext as Game;
            var dataContext = DataContext as ManageGamesViewModel;
            dataContext?.OpenManageMods(game);
        }
    }
}