using Avalonia.Controls;
using Avalonia.Input;
using ModStation.Avalonia.ViewModels;

namespace ModStation.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Games_PointerPressed(object sender, PointerPressedEventArgs e)
{
    if (sender is TextBlock textBlock) 
    {
        ((MainWindowViewModel)DataContext!).OpenGamesView(); 
    }
}
}