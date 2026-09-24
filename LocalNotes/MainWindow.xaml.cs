using System.Runtime.InteropServices;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;

namespace LocalNotes;

public sealed partial class MainWindow : Window
{
    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(IntPtr hWnd);

    private bool _isClosing;
    private bool _canClose;

    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.SetIcon("Assets/AppIcon.ico");
        ResizeWindow(1000, 900);
        RootFrame.Navigate(typeof(MainPage));
        AppWindow.Closing += OnClosing;
    }

    private void ResizeWindow(int widthDip, int heightDip)
    {
        // AppWindow sizes use physical pixels, while XAML uses effective pixels.
        IntPtr hwnd = Win32Interop.GetWindowFromWindowId(AppWindow.Id);
        double scale = GetDpiForWindow(hwnd) / 96.0;
        int width = (int)Math.Ceiling(widthDip * scale);
        int height = (int)Math.Ceiling(heightDip * scale);
        RectInt32? workArea = DisplayArea
            .GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest)
            ?.WorkArea;
        if (workArea is RectInt32 area)
        {
            int margin = (int)Math.Ceiling(32 * scale);
            width = Math.Min(width, Math.Max(1, area.Width - margin));
            height = Math.Min(height, Math.Max(1, area.Height - margin));
        }

        AppWindow.Resize(new SizeInt32(width, height));
    }

    private async void OnClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (_canClose)
        {
            return;
        }
        args.Cancel = true;
        if (_isClosing)
        {
            return;
        }
        _isClosing = true;
        await ((MainPage)RootFrame.Content).ViewModel.ShutdownAsync();
        _canClose = true;
        Close();
    }
}
