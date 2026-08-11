using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CalendarUI.Demo.ViewModels;

namespace CalendarUI.Demo.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
