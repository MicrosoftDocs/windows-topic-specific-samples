using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using WinUINotes.Services;

namespace WinUINotes.Tests.Fakes
{
    internal class FakeFileService : IFileService
    {
        private Dictionary<string, string> fileStorage = [];

        public async Task CreateOrUpdateFileAsync(string filename, string contents)
        {
            if (fileStorage.ContainsKey(filename))
            {
                fileStorage[filename] = contents;
            }
            else
            {
                fileStorage.Add(filename, contents);
            }

            await Task.Delay(10); // Simulate some async work
        }

        public async Task DeleteFileAsync(string filename)
        {
            if (fileStorage.ContainsKey(filename))
            {
                fileStorage.Remove(filename);
            }

            await Task.Delay(10); // Simulate some async work
        }

        public bool FileExists(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("Filename cannot be null or empty", nameof(filename));
            }

            if (fileStorage.ContainsKey(filename))
            {
                return true;
            }

            return false;
        }

        public IStorageFolder GetLocalFolder()
        {
            return new FakeStorageFolder(fileStorage);
        }

        public async Task<IReadOnlyList<IStorageItem>> GetStorageItemsAsync()
        {
            await Task.Delay(10);
            return GetStorageItemsInternal();
        }

        public async Task<IReadOnlyList<IStorageItem>> GetStorageItemsAsync(IStorageFolder storageFolder)
        {
            await Task.Delay(10);
            return GetStorageItemsInternal();
        }

        private IReadOnlyList<IStorageItem> GetStorageItemsInternal()
        {
            return fileStorage.Keys.Select(filename => CreateFakeStorageItem(filename)).ToList();
        }

        private IStorageItem CreateFakeStorageItem(string filename)
        {
            return new FakeStorageFile(filename);
        }

        public async Task<string> GetTextFromFileAsync(IStorageFile file)
        {
            await Task.Delay(10);

            if (fileStorage.ContainsKey(file.Name))
            {
                return fileStorage[file.Name];
            }

            return string.Empty;
        }
    }
}
