using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using WinUINotes.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUINotes.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotePage : Page
    {
        private Note? noteModel;

        public NotePage()
        {
            this.InitializeComponent();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (noteModel is not null)
            {
                await noteModel.SaveAsync();
            }
        }

        private async void SaveCloseButton_Click(SplitButton sender, SplitButtonClickEventArgs args)
        {
            if (noteModel is not null)
            {
                await noteModel.SaveAsync();
                Frame.Navigate(typeof(AllNotesPage), noteModel);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (noteModel is not null)
            {
                if (noteModel.State == NoteState.Unset)
                {
                    // If the note is new, doesn't have any edits,
                    // and hasn't been saved, just call GoBack.
                    // There's no need to pass back the noteModel.
                    if (Frame.CanGoBack == true)
                    {
                        Frame.GoBack();
                    }
                }
                else
                {
                    // If the note has been saved before, then delete it
                    // and navigate back to the AllNotesPage passing the
                    // noteModel with its Deleted state.
                    await noteModel.DeleteAsync();
                    Frame.Navigate(typeof(AllNotesPage), noteModel);
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Note note)
            {
                noteModel = note;
            }
            else
            {
                noteModel = new Note();
            }
        }

        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (noteModel?.State == NoteState.Unsaved)
            {
                e.Cancel = true;
                ContentDialog dialog = new ContentDialog();
                dialog.XamlRoot = this.XamlRoot;
                dialog.Title = "Save your work?";
                dialog.PrimaryButtonText = "Save";
                dialog.SecondaryButtonText = "Don't Save";
                dialog.CloseButtonText = "Cancel";
                dialog.DefaultButton = ContentDialogButton.Primary;

                ContentDialogResult result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    // Save changes.
                    await noteModel.SaveAsync();
                    Frame.Navigate(typeof(AllNotesPage), noteModel);
                }
                else if (result == ContentDialogResult.Secondary)
                {
                    // Discard changes.
                    while (NoteEditor.CanUndo)
                    {
                        NoteEditor.Undo();
                    }
                    // Call Focus because the Text binding isn't updated on Undo.
                    // But it is updated when the control gets focus.
                    NoteEditor.Focus(FocusState.Programmatic);
                    noteModel.State = NoteState.Saved;
                    if (Frame.CanGoBack)
                    {
                        Frame.GoBack();
                    }
                }

            }
        }
    }
}
