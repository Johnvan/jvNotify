using jvNotify.Views;

namespace jvNotify
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(NoteEntryPage), typeof(NoteEntryPage)); // this is called when the ADD of the NotesPage ispressed
        }
    }
}