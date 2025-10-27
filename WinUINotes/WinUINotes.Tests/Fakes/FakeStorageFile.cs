using System;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace WinUINotes.Tests.Fakes
{
    internal class FakeStorageFile : IStorageFile
    {
        private string name;


        public FakeStorageFile(string name)
        {
            this.name = name;
        }

        public string ContentType => throw new NotImplementedException();

        public string FileType => throw new NotImplementedException();

        public FileAttributes Attributes => throw new NotImplementedException();

        public DateTimeOffset DateCreated => throw new NotImplementedException();

        public string Name => name;

        public string Path => throw new NotImplementedException();

        public IAsyncAction CopyAndReplaceAsync(IStorageFile fileToReplace)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<StorageFile> CopyAsync(IStorageFolder destinationFolder)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<StorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<StorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option)
        {
            throw new NotImplementedException();
        }

        public IAsyncAction DeleteAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncAction DeleteAsync(StorageDeleteOption option)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<BasicProperties> GetBasicPropertiesAsync()
        {
            throw new NotImplementedException();
        }

        public bool IsOfType(StorageItemTypes type)
        {
            if (type == StorageItemTypes.File)
            {
                return true;
            }

            return false;
        }

        public IAsyncAction MoveAndReplaceAsync(IStorageFile fileToReplace)
        {
            throw new NotImplementedException();
        }

        public IAsyncAction MoveAsync(IStorageFolder destinationFolder)
        {
            throw new NotImplementedException();
        }

        public IAsyncAction MoveAsync(IStorageFolder destinationFolder, string desiredNewName)
        {
            throw new NotImplementedException();
        }

        public IAsyncAction MoveAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<IRandomAccessStream> OpenAsync(FileAccessMode accessMode)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<IRandomAccessStreamWithContentType> OpenReadAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<IInputStream> OpenSequentialReadAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<StorageStreamTransaction> OpenTransactedWriteAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncAction RenameAsync(string desiredName)
        {
            throw new NotImplementedException();
        }

        public IAsyncAction RenameAsync(string desiredName, NameCollisionOption option)
        {
            throw new NotImplementedException();
        }
    }
}
