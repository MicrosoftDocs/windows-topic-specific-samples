using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WinUINotes.Tests.Fakes;

namespace WinUINotes.Tests
{
    [TestClass]
    public partial class NoteTests
    {
        [TestMethod]
        public void TestCreateUnsavedNote()
        {
            var noteVm = new ViewModels.NoteViewModel(new FakeFileService());
            Assert.IsNotNull(noteVm);
            Assert.IsGreaterThan(DateTime.Now.AddHours(-1), noteVm.Date);
            Assert.EndsWith(".txt", noteVm.Filename);
            Assert.StartsWith("notes", noteVm.Filename);
            noteVm.Text = "Sample Note";
            Assert.AreEqual("Sample Note", noteVm.Text);
            noteVm.SaveCommand.Execute(null);
            Assert.AreEqual("Sample Note", noteVm.Text);
        }
    }
}
