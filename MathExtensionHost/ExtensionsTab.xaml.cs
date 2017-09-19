using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MathExtensionHost
{
    /// <summary>
    /// Displays the extensions and whether they are enabled or not. Also allows the user to remove extensions.
    /// </summary>
    public sealed partial class ExtensionsTab : Page
    {
        public ObservableCollection<Extension> Items = null; // The UI binds against the extensions

        public ExtensionsTab()
        {
            this.InitializeComponent();

            Items = AppData.ExtensionManager.Extensions; // The AppData object holds global state such as the collection of extensions
            this.DataContext = Items;
        }

        /// <summary>
        /// Enable an extension
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            Extension ext = cb.DataContext as Extension;
            if (!ext.Enabled)
            {
                await ext.Enable();
            }
        }

        /// <summary>
        /// Disable an extension
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            Extension ext = cb.DataContext as Extension;
            if (ext.Enabled)
            {
                ext.Disable();
            }
        }

        /// <summary>
        /// Remove an extension from the system if the user OKs it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            // remove the package
            Button btn = sender as Button;
            Extension ext = btn.DataContext as Extension;
            AppData.ExtensionManager.RemoveExtension(ext);
        }
    }
}
