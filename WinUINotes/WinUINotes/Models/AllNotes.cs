using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using WinUINotes.Models;

namespace WinUINotes.Models
{
    public class AllNotes
    {
        public ObservableCollection<Note> Notes { get; set; } =
                                    new ObservableCollection<Note>();

        public AllNotes()
        {
            LoadNotes();
        }

        public async void LoadNotes()
        {
            Notes.Clear();
            // Get the folder where the notes are stored.
            StorageFolder storageFolder =
                          ApplicationData.Current.LocalFolder;
            await GetFilesInFolderAsync(storageFolder);
        }

        private async Task GetFilesInFolderAsync(StorageFolder folder)
        {
            // Each StorageItem can be either a folder or a file.
            IReadOnlyList<IStorageItem> storageItems =
                                        await folder.GetItemsAsync();
            foreach (IStorageItem item in storageItems)
            {
                if (item.IsOfType(StorageItemTypes.Folder))
                {
                    // Recursively get items from subfolders.
                    await GetFilesInFolderAsync((StorageFolder)item);
                }
                else if (item.IsOfType(StorageItemTypes.File))
                {
                    StorageFile file = (StorageFile)item;
                    Note note = new Note()
                    {
                        Filename = file.Name,
                        Text = await FileIO.ReadTextAsync(file),
                        Date = file.DateCreated.DateTime
                    };
                    Notes.Add(note);
                }
            }
        }
    }
}
