using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WinUINotes.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUINotes.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AllNotesPage : Page
    {
        private AllNotes notesModel = new AllNotes();

        public AllNotesPage()
        {
            this.InitializeComponent();
        }

        private void NewNoteButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NotePage));
        }

        private void ItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
        {
            Frame.Navigate(typeof(NotePage), args.InvokedItem);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Note note)
            {
                if (note.State == NoteState.Deleted)
                {
                    notesModel.RemoveNote(note);
                }
                else if (!notesModel.Notes.Contains(note))
                {
                    notesModel.AddNote(note);
                }
                Frame.BackStack.Clear();
            }
        }
    }
}
