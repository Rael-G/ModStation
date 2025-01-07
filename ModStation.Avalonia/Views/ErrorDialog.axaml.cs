using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ModStation.Avalonia.Views;

public partial class ErrorDialog : Window
{
    public string? FirstDescription
    {
        get => FirstDescriptionText.Text;
        set => FirstDescriptionText.Text = value;
    }

    public string? SecondDescription
    {
        get => SecondDescriptionText.Text;
        set => SecondDescriptionText.Text = value;
    }

    public ErrorDialog()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}