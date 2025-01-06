using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ModManager.Core.Entities;
using ModStation.Avalonia.ViewModels;

namespace ModStation.Avalonia.Views;

public partial class ManageModsView : UserControl
{
    public ManageModsView()
    {
        InitializeComponent();
    }

    private void ModEnable_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (sender is TextBlock textBlock && textBlock.DataContext is Mod mod) 
        {
            ((ManageModsViewModel)DataContext!).ToggleModCommand.Execute(mod); 
        }
    }
}