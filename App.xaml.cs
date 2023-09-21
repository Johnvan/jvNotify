using jvNotify.Data;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;

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
                    database = new NoteDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyNotes3.db3"));
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();

#if IOS || ANDROID
            // Local Notification tap event listener - Not supported on Windows - yet
            LocalNotificationCenter.Current.NotificationActionTapped += OnLocalNotificationTapped;
#endif
            MainPage = new AppShell();
        }

        private void OnLocalNotificationTapped(NotificationEventArgs e)
        {
            // your code goes here
            //DisplayAlert("Hello", "mmm", "OK");
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