//  ---------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace StandardUICommandSample
{
    /// <summary>
    /// A list item class with an associated Command and Command text.
    /// </summary>
    public class ListItemData
    {
        public String Text { get; set; }
        public ICommand Command { get; set; }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// ListView item collection.
        /// </summary>
        ObservableCollection<ListItemData> collection = 
            new ObservableCollection<ListItemData>();

        /// <summary>
        /// Handler for the layout Grid control load event.
        /// </summary>
        /// <param name="sender">Source of the control loaded event</param>
        /// <param name="e">Event args for the loaded event</param>
        private void ControlExample_Loaded(object sender, RoutedEventArgs e)
        {
            // Create the standard Delete command.
            var deleteCommand = new StandardUICommand(StandardUICommandKind.Delete);
            deleteCommand.ExecuteRequested += DeleteCommand_ExecuteRequested;

            DeleteFlyoutItem.Command = deleteCommand;

            for (var i = 0; i < 5; i++)
            {
                collection.Add(
                    new ListItemData {
                        Text = "List item " + i.ToString(),
                        Command = deleteCommand });
            }
        }

        /// <summary>
        /// Handler for the ListView control load event.
        /// </summary>
        /// <param name="sender">Source of the control loaded event</param>
        /// <param name="e">Event args for the loaded event</param>
        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            var listView = (ListView)sender;
            // Populate the ListView with the item collection.
            listView.ItemsSource = collection;
        }

        /// <summary>
        /// Handler for the Delete command.
        /// </summary>
        /// <param name="sender">Source of the command event</param>
        /// <param name="e">Event args for the command event</param>
        private void DeleteCommand_ExecuteRequested(
            XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            // If possible, remove specfied item from collection.
            if (args.Parameter != null)
            {
                foreach (var i in collection)
                {
                    if (i.Text == (args.Parameter as string))
                    {
                        collection.Remove(i);
                        return;
                    }
                }
            }
            if (ListViewRight.SelectedIndex != -1)
            {
                collection.RemoveAt(ListViewRight.SelectedIndex);
            }
        }

        /// <summary>
        /// Handler for the ListView selection changed event.
        /// </summary>
        /// <param name="sender">Source of the selection changed event</param>
        /// <param name="e">Event args for the selection changed event</param>
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewRight.SelectedIndex != -1)
            {
                var item = collection[ListViewRight.SelectedIndex];
            }
        }

        /// <summary>
        /// Handler for the pointer entered event.
        /// Displays the delete item "hover" buttons.
        /// </summary>
        /// <param name="sender">Source of the pointer entered event</param>
        /// <param name="e">Event args for the pointer entered event</param>
        private void ListViewSwipeContainer_PointerEntered(
            object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == 
                Windows.Devices.Input.PointerDeviceType.Mouse || 
                e.Pointer.PointerDeviceType == 
                Windows.Devices.Input.PointerDeviceType.Pen)
            {
                VisualStateManager.GoToState(
                    sender as Control, "HoverButtonsShown", true);
            }
        }

        /// <summary>
        /// Handler for the pointer exited event.
        /// Hides the delete item "hover" buttons.
        /// </summary>
        /// <param name="sender">Source of the pointer exited event</param>
        /// <param name="e">Event args for the pointer exited event</param>

        private void ListViewSwipeContainer_PointerExited(
            object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(
                sender as Control, "HoverButtonsHidden", true);
        }
    }
}