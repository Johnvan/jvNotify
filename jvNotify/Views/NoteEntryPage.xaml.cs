using Plugin.LocalNotification;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using jvNotify.Models;

namespace jvNotify.Views
{
  [QueryProperty(nameof(ItemId), nameof(ItemId))]
  public partial class NoteEntryPage : ContentPage
  {
    private DateTime SelDate;
    private Note note = new Note();
    //private TimeSpan SelTime;
    private int notificationNumber = 0;

    public string ItemId
    {
      set => LoadNote(value);
    }

    public NoteEntryPage()
    {
      InitializeComponent();

    }

    private async void LoadNote(string itemId)
    {
      try
      {
        BindingContext = note;
        int id = Convert.ToInt32(itemId);
        note = await App.Database.GetNoteAsync(id);
        editThis.Text = note.Text;
        _startDatePicker.Date = (DateTime)note.StartDate;
        _startTimePicker.Time = (TimeSpan)note.StartDate.TimeOfDay;
        _onGoPicker.SelectedIndex = note.OnGo;
        _warnPicker.SelectedIndex = note.Warn;
        _typePicker.SelectedIndex = note.TypeDate;
        _Warn1.Text = note.Min.ToString();
        _Warn2.Text = note.Hrs.ToString();
      }
      catch (Exception)
      {
        Console.WriteLine("Failed to load note.");
      }
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
      SelDate = _startDatePicker.Date + _startTimePicker.Time;
      string NoValue = "00:00:00";  // If No Time was selected then don't save Date
      string InTime = _startTimePicker.Time.ToString();
      note.Text = editThis.Text;
      note.StartDate = SelDate;
      note.OnGo = _onGoPicker.SelectedIndex;
      note.Warn = _warnPicker.SelectedIndex;
      note.TypeDate = _typePicker.SelectedIndex;
      note.Min = Convert.ToInt32(_Warn1.Text);
      note.Hrs = Convert.ToInt32(_Warn2.Text);
      if (InTime == NoValue)
      {
        note.StartDate = DateTime.MinValue;
      }
      if (!string.IsNullOrWhiteSpace(note.Text))  // Don't Save anything if there is not a Description
      {
        if (InTime != NoValue) { SetupNotification(SelDate, note.Text); }
        await App.Database.SaveNoteAsync(note); // Save to Database
      }
      // Navigate backwards
      await Shell.Current.GoToAsync("..");
    }

    private async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
      await App.Database.DeleteNoteAsync(note);
      // Navigate backwards
      await Shell.Current.GoToAsync("..");
    }

    private void OnTimeChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      //if (e.PropertyName == "Time")
      //{
      //  SelTime = _startTimePicker.Time;
      //}
    }

    private void OnDateSelected(object sender, PropertyChangedEventArgs e)
    {
      SelDate = _startDatePicker.Date;
    }

    ////////////////// The following are the Routines for Notifications ///////////////
    private void ShowNotification(string title, string message)
    {
      Device.BeginInvokeOnMainThread(async () =>
      {
        Label msg = new Label()
        {
          Text = $"Notification Received:\nTitle: {title}\nMessage: {message}"
        };
        await DisplayAlert(msg.Text, "mmm", "OK");
        //editThis.Text = msg.Text;
      });
    }

    private async void SetupNotification(DateTime theDate, string outMessage)
    {
      TimeSpan T1 = new TimeSpan(0, Convert.ToInt32(_Warn1.Text), 0);  // Hour/ Min/ Sec
      DateTime alarm1 = theDate.Subtract(T1);    // Minutes;
      TimeSpan T2 = new TimeSpan(Convert.ToInt32(_Warn2.Text), 0, 0);  // Hour/ Min/ Sec
      DateTime alarm2 = theDate.Subtract(T2);    // Hours

      var notification = new NotificationRequest  // Minutes Alarm
      { 
        NotificationId = notificationNumber++,
        Description = $"Dont't Forget: {outMessage} at {theDate}",
        Title = $"Appointment notification!",
        ReturningData = "Dummy data", // Returning data when tapped on notification.
        Schedule = { NotifyTime = alarm1 }
      };
      await NotificationCenter.Current.Show(notification);

      var notification2 = new NotificationRequest  // Hours Alarm
      {
        NotificationId = notificationNumber++,
        Description = $"Dont't Forget: {outMessage} at {theDate}",
        Title = $"Appointment notification!",
        ReturningData = "Dummy data", // Returning data when tapped on notification.
        Schedule = { NotifyTime = alarm2 }
      };
      await NotificationCenter.Current.Show(notification2);
    }
  }
}