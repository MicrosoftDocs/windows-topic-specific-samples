using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MathExtensionHost
{
    /// <summary>
    /// Handles the hamburger menu on the MainPage
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            AppData.ExtensionManager.Initialize(Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher); // The extension manager can update UI, so pass it the UI dispatcher to use for UI updates
            SplitViewFrame.Navigate(typeof(CalculateTab), null); // start with the Calculate page visible
        }

        /// <summary>
        /// Toggle the SplitView's pane to show/hide descriptions for the buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            TheSplitView.IsPaneOpen = !TheSplitView.IsPaneOpen;
        }

        /// <summary>
        /// Open the Calculate page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            SplitViewFrame.Navigate(typeof(CalculateTab), null);
        }

        /// <summary>
        /// Open the Extensions page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Extensions_Click(object sender, RoutedEventArgs e)
        {
            SplitViewFrame.Navigate(typeof(ExtensionsTab), null);
        }
    }
}
