using Microsoft.UI.Xaml;

namespace LocalNotes;

public partial class App : Application
{
    private Window? _window;

    public App()
    {
        InitializeComponent();
        HighContrastAdjustment = ApplicationHighContrastAdjustment.Auto;
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }
}
