using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace WinUINotes.Services
{
    public class WindowsFileService : IFileService
    {
        public StorageFolder storageFolder;

        public WindowsFileService(IStorageFolder storageFolder)
        {
            this.storageFolder = (StorageFolder)storageFolder;

            if (this.storageFolder is null)
            {
                throw new ArgumentException("storageFolder must be of type StorageFolder", nameof(storageFolder));
            }
        }

        public async Task CreateOrUpdateFileAsync(string filename, string contents)
        {
            // Save the note to a file.
            StorageFile storageFile = (StorageFile)await storageFolder.TryGetItemAsync(filename);
            if (storageFile is null)
            {
                storageFile = await storageFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            }
            await FileIO.WriteTextAsync(storageFile, contents);
        }

        public async Task DeleteFileAsync(string filename)
        {
            // Delete the note from the file system.
            StorageFile storageFile = (StorageFile)await storageFolder.TryGetItemAsync(filename);
            if (storageFile is not null)
            {
                await storageFile.DeleteAsync();
            }
        }

        public bool FileExists(string filename)
        {
            StorageFile storageFile = (StorageFile)storageFolder.TryGetItemAsync(filename).AsTask().Result;
            return storageFile is not null;
        }

        public IStorageFolder GetLocalFolder()
        {
            return storageFolder;
        }

        public async Task<IReadOnlyList<IStorageItem>> GetStorageItemsAsync()
        {
            return await storageFolder.GetItemsAsync();
        }

        public async Task<IReadOnlyList<IStorageItem>> GetStorageItemsAsync(IStorageFolder folder)
        {
            return await folder.GetItemsAsync();
        }

        public async Task<string> GetTextFromFileAsync(IStorageFile file)
        {
            return await FileIO.ReadTextAsync(file);
        }
    }
}
