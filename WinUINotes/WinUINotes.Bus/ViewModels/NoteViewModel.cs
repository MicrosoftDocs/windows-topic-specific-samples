using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Threading.Tasks;
using WinUINotes.Models;
using WinUINotes.Services;

namespace WinUINotes.ViewModels
{
    public partial class NoteViewModel : ObservableObject
    {
        private Note note;
        private IFileService fileService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
        private string filename = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string text = string.Empty;

        [ObservableProperty]
        private DateTime date = DateTime.Now;

        public NoteViewModel(IFileService fileService)
        {
            this.fileService = fileService;
            this.note = new Note(fileService);
            this.Filename = note.Filename;
        }

        public void InitializeForExistingNote(Note note)
        {
            this.note = note;
            this.Filename = note.Filename;
            this.Text = note.Text;
            this.Date = note.Date;
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task Save()
        {
            note.Filename = this.Filename;
            note.Text = this.Text;
            note.Date = this.Date;
            await note.SaveAsync();

            // Check if the DeleteCommand can now execute
            // (it can if the file now exists)
            DeleteCommand.NotifyCanExecuteChanged();
        }

        private bool CanSave()
        {
            return note is not null
                && !string.IsNullOrWhiteSpace(this.Text)
                && !string.IsNullOrWhiteSpace(this.Filename);
        }

        [RelayCommand(CanExecute = nameof(CanDelete))]
        private async Task Delete()
        {
            await note.DeleteAsync();
            note = new Note(fileService);
            // Send a message from some other module
            WeakReferenceMessenger.Default.Send(new NoteDeletedMessage(note));
        }

        private bool CanDelete()
        {
            // Note: This is to illustrate how commands can be
            // enabled or disabled.
            // In a real application, you shouldn't perform
            // file operations in your CanExecute logic.
            return note is not null
                && !string.IsNullOrWhiteSpace(this.Filename)
                && this.note.NoteFileExists();
        }
    }
}
