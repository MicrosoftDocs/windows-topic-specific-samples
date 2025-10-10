using CommunityToolkit.Mvvm.Messaging.Messages;
using WinUINotes.Models;

namespace WinUINotes
{
    public class NoteDeletedMessage : ValueChangedMessage<Note>
    {
        public NoteDeletedMessage(Note note) : base(note)
        {
        }
    }
}
