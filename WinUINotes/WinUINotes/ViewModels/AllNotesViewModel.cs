using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WinUINotes.Models;

namespace WinUINotes.ViewModels
{
    public partial class AllNotesViewModel : ObservableObject
    {
        private readonly AllNotes allNotes;

        [ObservableProperty]
        private ObservableCollection<Note> notes;

        public AllNotesViewModel()
        {
            allNotes = new AllNotes();
            notes = new ObservableCollection<Note>();
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            await allNotes.LoadNotes();
            Notes.Clear();
            foreach (var note in allNotes.Notes)
            {
                Notes.Add(note);
            }
        }
    }
}
