using CommunityToolkit.Mvvm.ComponentModel;

namespace TaskTally.Models;

public partial class TaskItem : ObservableObject
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [ObservableProperty]
    public partial string Title { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsComplete { get; set; }
}
