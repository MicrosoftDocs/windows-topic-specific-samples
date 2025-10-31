using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace WinUINotes.Tests.Fakes
{
    internal class FakeStorageFolder : IStorageFolder
    {
        private string name;
        private Dictionary<string, string> fileStorage = [];

        public FakeStorageFolder(Dictionary<string, string> files)
        {
            fileStorage = files;
        }

        public FileAttributes Attributes => throw new NotImplementedException();

        public DateTimeOffset DateCreated => throw new NotImplementedException();

        public string Name => name;

        public string Path => throw new NotImplementedException();

        public IAsyncOperation<StorageFile> CreateFileAsync(string desiredName)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<StorageFile> CreateFileAsync(string desiredName, CreationCollisionOption options)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<StorageFolder> CreateFolderAsync(string desiredName)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<StorageFolder> CreateFolderAsync(string desiredName, CreationCollisionOption options)
        {
            throw new NotImplementedException();
        }

        public IAsyncAction DeleteAsync()
        {
            // For simplicity, do nothing.
            return AsyncInfo.Run(async cancelToken =>
            {
                // No operation
            });
        }

        public IAsyncAction DeleteAsync(StorageDeleteOption option)
        {
            // For simplicity, do nothing.
            return AsyncInfo.Run(async cancelToken =>
            {
                // No operation
            });
        }

        public IAsyncOperation<BasicProperties> GetBasicPropertiesAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<StorageFile> GetFileAsync(string name)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<IReadOnlyList<StorageFile>> GetFilesAsync()
        {
            return AsyncInfo.Run<IReadOnlyList<StorageFile>>(async cancelToken =>
            {
                List<IStorageFile> files = new();
                foreach (var filename in fileStorage.Keys)
                {
                    files.Add(new FakeStorageFile(filename));
                }
                return (IReadOnlyList<StorageFile>)files;
            });
        }

        public IAsyncOperation<StorageFolder> GetFolderAsync(string name)
        {
            // no folders to return. return null.
            return AsyncInfo.Run<StorageFolder>(async cancelToken =>
            {
                return null;
            });
        }

        public IAsyncOperation<IReadOnlyList<StorageFolder>> GetFoldersAsync()
        {
            // no folders to return. return empty list.
            return AsyncInfo.Run<IReadOnlyList<StorageFolder>>(async cancelToken =>
            {
                List<IStorageFolder> folders = new();
                return (IReadOnlyList<StorageFolder>)folders;
            });
        }

        public IAsyncOperation<IStorageItem> GetItemAsync(string name)
        {
            // Check if the name exists in the file storage
            return AsyncInfo.Run<IStorageItem>(async cancelToken =>
            {
                if (fileStorage.ContainsKey(name))
                {
                    return new FakeStorageFile(name);
                }
                else
                {
                    return null;
                }
            });
        }

        public IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync()
        {
            // Return all files as IStorageItem
            return AsyncInfo.Run<IReadOnlyList<IStorageItem>>(async cancelToken =>
            {
                List<IStorageItem> items = new();
                foreach (var filename in fileStorage.Keys)
                {
                    items.Add(new FakeStorageFile(filename));
                }
                return (IReadOnlyList<IStorageItem>)items;
            });
        }

        public bool IsOfType(StorageItemTypes type)
        {
            if (type == StorageItemTypes.Folder)
            {
                return true;
            }

            return false;
        }

        public IAsyncAction RenameAsync(string desiredName)
        {
            // For simplicity, just change the name property.
            return AsyncInfo.Run(async cancelToken =>
            {
                name = desiredName;
            });
        }

        public IAsyncAction RenameAsync(string desiredName, NameCollisionOption option)
        {
            // For simplicity, just change the name property.
            return AsyncInfo.Run(async cancelToken =>
            {
                name = desiredName;
            });
        }
    }
}
