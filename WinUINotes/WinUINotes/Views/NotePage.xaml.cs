using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WinUINotes.Models;
using WinUINotes.ViewModels;

namespace WinUINotes.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotePage : Page
    {
        private NoteViewModel? noteVm;

        public NotePage()
        {
            this.InitializeComponent();
        }

        public void RegisterForDeleteMessages()
        {
            WeakReferenceMessenger.Default.Register<NoteDeletedMessage>(this, (r, m) =>
            {
                if (Frame.CanGoBack)
                {
                    Frame.GoBack();
                }
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            noteVm = App.Current.Services.GetService<NoteViewModel>();
            RegisterForDeleteMessages();

            if (e.Parameter is Note note && noteVm is not null)
            {
                noteVm.InitializeForExistingNote(note);
            }
        }
    }
}
