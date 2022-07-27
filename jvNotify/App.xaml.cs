
using System;
using System.IO;
using Xamarin.Forms;
using jvNotify.Data;

namespace jvNotify
{
  public partial class App : Application
  {
    private static NoteDatabase database;

    // Create the database connection as a singleton.
    public static NoteDatabase Database
    {
      get
      {
        if (database == null)
        {
          database = new NoteDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyNotes2.db3"));
        }
        return database;
      }
    }

    public App()
    {
      InitializeComponent();
      MainPage = new AppShell();
    }

    protected override void OnStart()
    {
    }

    protected override void OnSleep()
    {
    }

    protected override void OnResume()
    {
    }
  }
}
