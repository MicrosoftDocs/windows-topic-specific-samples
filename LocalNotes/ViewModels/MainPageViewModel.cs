using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalNotes.Services;

namespace LocalNotes.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private const string NoteChangedStatus = "Note changed. Choose Summarize to create a new summary.";
    private readonly LocalSummarizer _summarizer = new();
    private bool _isClosing;
    private int _noteRevision;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SummarizeCommand))]
    public partial string Note { get; set; } = "";

    [ObservableProperty]
    public partial string Summary { get; set; } = "";

    [ObservableProperty]
    public partial string Status { get; set; } = "Model not prepared. Choose Download and prepare to begin. Cached downloads are reused.";

    [ObservableProperty]
    public partial string ModelId { get; set; } = "Qwen2.5 0.5B • CPU • 256 output tokens maximum";

    [ObservableProperty]
    public partial string Error { get; set; } = "";

    [ObservableProperty]
    public partial bool HasError { get; set; }

    [ObservableProperty]
    public partial double DownloadProgress { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrepareCommand))]
    [NotifyCanExecuteChangedFor(nameof(SummarizeCommand))]
    public partial bool IsBusy { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrepareCommand))]
    [NotifyCanExecuteChangedFor(nameof(SummarizeCommand))]
    public partial bool IsReady { get; set; }

    private bool CanPrepare() => !_isClosing && !IsBusy && !IsReady;
    private bool CanSummarize() => !_isClosing && !IsBusy && IsReady
        && !string.IsNullOrWhiteSpace(Note) && Note.Length <= LocalSummarizer.MaxNoteLength;

    partial void OnNoteChanged(string value)
    {
        _noteRevision++;
        Summary = "";
        if (SummarizeCommand.IsRunning)
        {
            Status = "Note changed. Canceling the previous summary…";
            SummarizeCommand.Cancel();
        }
        else if (IsReady)
        {
            Status = NoteChangedStatus;
        }
    }

    [RelayCommand(CanExecute = nameof(CanPrepare))]
    private async Task PrepareAsync()
    {
        IsBusy = true;
        HasError = false;
        DownloadProgress = 0;
        var status = new Progress<string>(value => Status = value);
        var progress = new Progress<double>(value => DownloadProgress = value);
        try
        {
            // Native initialization/loading may do synchronous work before its first await.
            ModelId = await Task.Run(() => _summarizer.PrepareAsync(status, progress));
            IsReady = true;
            Status = "Ready. Write a note, then choose Summarize.";
        }
        catch (Exception error) when (LocalSummarizer.IsOperationalError(error))
        {
            ShowError(error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanSummarize), IncludeCancelCommand = true)]
    private async Task SummarizeAsync(CancellationToken cancellationToken)
    {
        IsBusy = true;
        HasError = false;
        Summary = "";
        Status = "Generating locally on CPU…";
        string note = Note; // Preserve a snapshot; never overwrite the original note.
        int revision = _noteRevision;
        var output = new Progress<string>(value =>
        {
            if (revision == _noteRevision && !cancellationToken.IsCancellationRequested)
            {
                Summary = value;
            }
        });
        try
        {
            string summary = await Task.Run(() => _summarizer.SummarizeAsync(note, output, cancellationToken));
            cancellationToken.ThrowIfCancellationRequested();
            if (revision == _noteRevision)
            {
                Summary = summary;
                Status = "Summary complete. Review it for accuracy.";
            }
            else
            {
                Status = NoteChangedStatus;
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            Summary = "";
            Status = revision == _noteRevision
                ? "Canceled. No summary was kept."
                : NoteChangedStatus;
        }
        catch (Exception error) when (LocalSummarizer.IsOperationalError(error))
        {
            Summary = "";
            ShowError(error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ShowError(Exception error)
    {
        Error = error.Message;
        HasError = true;
        Status = "Could not complete the operation. Check the error and try again.";
    }

    public async Task ShutdownAsync()
    {
        _isClosing = true;
        PrepareCommand.NotifyCanExecuteChanged();
        SummarizeCommand.NotifyCanExecuteChanged();
        SummarizeCommand.Cancel();
        if (PrepareCommand.IsRunning)
        {
            return;
        }
        if (PrepareCommand.ExecutionTask is { } preparing)
        {
            await preparing;
        }
        if (SummarizeCommand.ExecutionTask is { } generating)
        {
            await generating;
        }
        Status = "Releasing the local model…";
        try
        {
            await Task.Run(async () => await _summarizer.DisposeAsync());
        }
        catch (Exception error) when (LocalSummarizer.IsOperationalError(error))
        {
            ShowError(error);
        }
    }
}
