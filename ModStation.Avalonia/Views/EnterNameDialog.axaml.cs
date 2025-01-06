using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace ModStation.Avalonia.Views;

public partial class EnterNameDialog : Window
{
    public string? NameText
    {
        get => NameTextBox.Text;
        set => NameTextBox.Text = value;
    }

    public EnterNameDialog()
    {
        InitializeComponent();
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        Close(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}