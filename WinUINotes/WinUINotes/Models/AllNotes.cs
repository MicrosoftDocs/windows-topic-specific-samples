using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using WinUINotes.Services;

namespace WinUINotes.Models
{
    public class AllNotes
    {
        private IFileService fileService;
        public ObservableCollection<Note> Notes { get; set; } = [];

        public AllNotes()
        {
            fileService = App.Current.Services.GetService<IFileService>();
        }

        public async Task LoadNotes()
        {
            Notes.Clear();
            await GetFilesInFolderAsync(fileService.GetLocalFolder());
        }

        private async Task GetFilesInFolderAsync(IStorageFolder folder)
        {
            // Each StorageItem can be either a folder or a file.
            IReadOnlyList<IStorageItem> storageItems =
                                        await fileService.GetStorageItemsAsync(folder);
            foreach (IStorageItem item in storageItems)
            {
                if (item.IsOfType(StorageItemTypes.Folder))
                {
                    // Recursively get items from subfolders.
                    await GetFilesInFolderAsync((IStorageFolder)item);
                }
                else if (item.IsOfType(StorageItemTypes.File))
                {
                    IStorageFile file = (IStorageFile)item;
                    Note note = new()
                    {
                        Filename = file.Name,
                        Text = await fileService.GetTextFromFileAsync(file),
                        Date = file.DateCreated.DateTime
                    };
                    Notes.Add(note);
                }
            }
        }
    }
}
