using System.Text.Json;
using System.Threading;
using TaskTally.Models;
using Windows.Storage;

namespace TaskTally.Services;

internal static class TaskStorage
{
    private const string FileName = "tasks.json";
    private static readonly SemaphoreSlim SaveLock = new(1, 1);

    public static async Task<IReadOnlyList<TaskItem>> LoadAsync()
    {
        StorageFile? file =
            await ApplicationData.Current.LocalFolder.TryGetItemAsync(FileName) as StorageFile;

        if (file is null)
        {
            return [];
        }

        string json = await FileIO.ReadTextAsync(file);
        return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? [];
    }

    public static async Task SaveAsync(IEnumerable<TaskItem> tasks)
    {
        await SaveLock.WaitAsync();
        try
        {
            string json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                FileName,
                CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteTextAsync(file, json);
        }
        finally
        {
            SaveLock.Release();
        }
    }
}
