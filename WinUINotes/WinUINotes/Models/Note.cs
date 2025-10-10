using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using WinUINotes.Services;

namespace WinUINotes.Models
{
    public class Note
    {
        private IFileService fileService;
        public string Filename { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;

        public Note()
        {
            Filename = "notes" + DateTime.Now.ToBinary().ToString() + ".txt";
            fileService = App.Current.Services.GetService<IFileService>();
        }

        public async Task SaveAsync()
        {
            await fileService.CreateOrUpdateFileAsync(Filename, Text);
        }

        public async Task DeleteAsync()
        {
            await fileService.DeleteFileAsync(Filename);
        }

        public bool NoteFileExists()
        {
            return fileService.FileExists(Filename);
        }
    }
}