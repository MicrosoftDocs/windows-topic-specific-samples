using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskTally.Models;
using TaskTally.Services;

namespace TaskTally.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private bool _isInitialized;

    public ObservableCollection<TaskItem> Tasks { get; } = [];

    [ObservableProperty]
    public partial string NewTaskTitle { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string StatusMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsStatusOpen { get; set; }

    [ObservableProperty]
    public partial bool HasError { get; set; }

    public string Summary =>
        Tasks.Count == 0
            ? "No tasks yet"
            : $"{Tasks.Count(task => !task.IsComplete)} of {Tasks.Count} remaining";

    public bool HasNoTasks => Tasks.Count == 0;

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;
        try
        {
            IReadOnlyList<TaskItem> tasks = await TaskStorage.LoadAsync();
            foreach (TaskItem task in tasks)
            {
                Tasks.Add(task);
            }

            RefreshTaskState();
        }
        catch (Exception ex) when (IsStorageError(ex))
        {
            ShowStatus($"Tasks couldn't be loaded: {ex.Message}", hasError: true);
        }
    }

    [RelayCommand(CanExecute = nameof(CanAddTask))]
    private async Task AddTaskAsync()
    {
        string title = NewTaskTitle.Trim();
        Tasks.Add(new TaskItem { Title = title });
        NewTaskTitle = string.Empty;
        RefreshTaskState();
        await SaveAsync($"Added \"{title}\".");
    }

    [RelayCommand]
    private async Task DeleteTaskAsync(TaskItem task)
    {
        Tasks.Remove(task);
        RefreshTaskState();
        await SaveAsync($"Deleted \"{task.Title}\".");
    }

    public async Task TaskCompletionChangedAsync()
    {
        RefreshTaskState();
        await SaveAsync("Updated tasks.");
    }

    private bool CanAddTask() => !string.IsNullOrWhiteSpace(NewTaskTitle);

    partial void OnNewTaskTitleChanged(string value)
    {
        AddTaskCommand.NotifyCanExecuteChanged();
    }

    private async Task SaveAsync(string successMessage)
    {
        try
        {
            await TaskStorage.SaveAsync(Tasks);
            ShowStatus(successMessage);
        }
        catch (Exception ex) when (IsStorageError(ex))
        {
            ShowStatus($"Changes couldn't be saved: {ex.Message}", hasError: true);
        }
    }

    private void RefreshTaskState()
    {
        OnPropertyChanged(nameof(Summary));
        OnPropertyChanged(nameof(HasNoTasks));
    }

    private void ShowStatus(string message, bool hasError = false)
    {
        StatusMessage = message;
        HasError = hasError;
        IsStatusOpen = true;
    }

    private static bool IsStorageError(Exception error) =>
        error is IOException or UnauthorizedAccessException or JsonException or COMException;
}
