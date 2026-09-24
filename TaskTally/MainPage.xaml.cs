using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TaskTally.Models;
using TaskTally.ViewModels;

namespace TaskTally;

public sealed partial class MainPage : Page
{
    public MainPageViewModel ViewModel { get; } = new();

    public MainPage()
    {
        InitializeComponent();
        Loaded += MainPage_Loaded;
    }

    public static string DeleteTaskName(string title) => $"Delete {title}";

    public static Visibility BoolToVisibility(bool value) =>
        value ? Visibility.Visible : Visibility.Collapsed;

    public static InfoBarSeverity StatusSeverity(bool hasError) =>
        hasError ? InfoBarSeverity.Error : InfoBarSeverity.Success;

    private async void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= MainPage_Loaded;
        await ViewModel.InitializeAsync();
        NewTaskTextBox.Focus(FocusState.Programmatic);
    }

    private async void TaskCheckBox_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.TaskCompletionChangedAsync();
    }

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: TaskItem task })
        {
            await ViewModel.DeleteTaskCommand.ExecuteAsync(task);
        }
    }
}
