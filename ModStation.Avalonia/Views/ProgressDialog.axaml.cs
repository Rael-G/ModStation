using Avalonia.Controls;

namespace ModStation.Avalonia.Views;

public partial class ProgressDialog : Window
{
    public string Text { get => MainText.Text!; set => MainText.Text = value; }
    public string Description { get => DescriptionText.Text!; set => DescriptionText.Text = value; }

    public ProgressDialog()
    {
        InitializeComponent();
    }
}