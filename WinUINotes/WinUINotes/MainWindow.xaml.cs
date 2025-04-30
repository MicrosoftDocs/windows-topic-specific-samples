using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUINotes
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            // Hide the default system title bar.
            ExtendsContentIntoTitleBar = true;
            // Replace system title bar with the WinUI TitleBar.
            SetTitleBar(AppTitleBar);

            // Create a new window presenter
            OverlappedPresenter presenter = OverlappedPresenter.Create();
            // Set the minimum width the window can be resized to
            presenter.PreferredMinimumWidth = 320;
            // Set the minimum height the window can be resized to
            presenter.PreferredMinimumHeight = 240;
            // Apply the presenter settings to the current AppWindow
            AppWindow.SetPresenter(presenter);
        }

        private void AppTitleBar_BackRequested(TitleBar sender, object args)
        {
            if (rootFrame.CanGoBack == true)
            {
                rootFrame.GoBack();
            }
        }
    }
}