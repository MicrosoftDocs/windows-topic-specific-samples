using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;

namespace WinUINotes.Models
{
    public class Note : INotifyPropertyChanged
    {
        private StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        public string Filename { get; set; } = string.Empty;
        //public string Text { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public NoteState State { get; set; } = NoteState.Unset;

        private string _text = string.Empty;
        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    State = NoteState.Unsaved;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Note()
        {
            Filename = "notes" + DateTime.Now.ToBinary().ToString() + ".txt";
        }

        public async Task SaveAsync()
        {
            // Save the note to a file.
            StorageFile noteFile = (StorageFile)await storageFolder.TryGetItemAsync(Filename);
            if (noteFile is null)
            {
                noteFile = await storageFolder.CreateFileAsync(Filename, CreationCollisionOption.ReplaceExisting);
            }
            await FileIO.WriteTextAsync(noteFile, Text);
            State = NoteState.Saved;
        }

        public async Task DeleteAsync()
        {
            // Delete the note from the file system.
            StorageFile noteFile = (StorageFile)await storageFolder.TryGetItemAsync(Filename);
            if (noteFile is not null)
            {
                await noteFile.DeleteAsync();
            }
            State = NoteState.Deleted;
        }
    }

    public enum NoteState
    {
        Unset = 0, Saved, Unsaved, Deleted
    }
}
