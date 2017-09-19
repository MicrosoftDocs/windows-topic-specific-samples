using System;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MathExtensionHost
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CalculateTab : Page
    {
        public CalculateTab()
        {
            this.InitializeComponent();
            DataContext = AppData.ExtensionManager.Extensions; // Populate the extension buttons using the collection of loaded extensions
        }

        /// <summary>
        /// Built-in functionality for the 'calculator'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            double x = Double.Parse(Arg1.Text);
            double y = Double.Parse(Arg2.Text);
            double result = x + y;
            Result.Text = result.ToString();
        }
        
        /// <summary>
        /// Invokes the extension associated with the button that was pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void InvokeExtension(object sender, RoutedEventArgs e)
        {
            // The contract that I've specified for math extensions is to expect arguments labeled arg1, arg2.
            ValueSet message = new ValueSet();
            message.Add("arg1", Double.Parse(Arg1.Text));
            message.Add("arg2", Double.Parse(Arg2.Text));

            // Get the extension to call based on the button pressed.
            // The extension is associated with the button's data context. 
            // The Items control builds each button from the collection of installed extensions
            Button btn = sender as Button;
            Extension ext = btn.DataContext as Extension;

            // Invoke the extension with the arguments in the ValueSet
            double result = await ext.Invoke(message);
            Result.Text = result.ToString();
        }
    }
}
