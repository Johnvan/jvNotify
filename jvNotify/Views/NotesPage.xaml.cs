using jvNotify.Models;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace jvNotify.Views
{
  public partial class NotesPage : ContentPage
  {
    private int notificationNumber = 0;

    public NotesPage()
    {
      InitializeComponent();
    }
    protected override async void OnAppearing()
    {
      base.OnAppearing();
      Note note = (Note)BindingContext;
      // Retrieve all the notes from the database, and set them as the
      // data source for the CollectionView.

      // Do the Motes Section
      List<Note> textlist = await App.Database.GetNotesAsync();
      collectionView1.ItemsSource = from n in textlist
                                    where n.StartDate == DateTime.MinValue  //No Date set for this record ie Notes Only
                                    orderby n.StartDate ascending
                                    select n;

      // Do the checks and adjustments for On Going calculations or Delete
      List<Note> dates = await App.Database.GetNotesAsync();
      IOrderedEnumerable<Note> Temp = from n in dates
                                      where n.StartDate != DateTime.MinValue  // Has a Date set for this record ie Appointments
                                      orderby n.StartDate ascending
                                      select n;
      foreach (Note result in Temp)
      {
        if ((result.StartDate > DateTime.MinValue) && (result.StartDate < DateTime.Now))
        {
          if (result.OnGo > 0)
          {
            if (result.TypeDate == 1)  // Same Day
            {
              result.StartDate = AddDay(result.StartDate, result.OnGo);  // Add Day
            }
            else if (result.TypeDate == 2)   // Same Date
            {
              result.StartDate = AddDate(result.StartDate, result.OnGo); // Add Date
            }
            else if (result.OnGo == 1)
            {
              result.StartDate = AddDate(result.StartDate, result.OnGo);   // Just add a Day
            }
            _ = await App.Database.SaveNoteAsync(result); // Save to Database
            SetupNotification(result.StartDate, result.Text, result.Min, result.Hrs); // , result.ID
          }
          else
          {
            result.StartDate = DateTime.MinValue; // This should move the record to the Notes area

            _ = await App.Database.SaveNoteAsync(result);
            //_ = await App.Database.DeleteNoteAsync(result);  // Delete old records
          }
        }
      }

      // Do the Appointments screen
      List<Note> reclist = await App.Database.GetNotesAsync();
      collectionView.ItemsSource = from n in reclist
                                   where n.StartDate > DateTime.MinValue
                                   orderby n.StartDate ascending
                                   select n;

      //// Check and Change back color of collectiveView if record Date is in current week
      //List<Note> reclist2 = await App.Database.GetNotesAsync();
      //collectionView.ItemsSource = from n in reclist2
      //                             where n.StartDate > DateTime.MinValue && n.StartDate < DateTime.Now.AddDays(7)
      //                             orderby n.StartDate ascending
      //                             select n;



      //List<Note> dates2 = await App.Database.GetNotesAsync();   
      // Do the Popup Messages
      IOrderedEnumerable<Note> Temp2 = from n in reclist
                                       where n.StartDate != DateTime.MinValue
                                       orderby n.StartDate ascending
                                       select n;
      foreach (Note result in Temp2)
      {
        if (result.StartDate <= DateTime.Now.AddDays(result.Warn)) // Do the Warning Messages
        {
          if (result.StartDate > DateTime.MinValue)
          {
            string sDate = "";
            DateTime startDate = result.StartDate;
            TimeSpan diffResult = startDate.Subtract(DateTime.Now);
            double x = Convert.ToDouble(diffResult.TotalDays);
            int aToday = Convert.ToInt32(DateTime.Now.Hour);
            int aDays = result.StartDate.DayOfYear - DateTime.Now.DayOfYear;
            int aHours = Convert.ToInt32(diffResult.Hours);
            if (aDays <= result.Warn) // Set for Alarm
            {
              if (result.StartDate.Date == DateTime.Now.Date)  // Alarm Day
              {
                sDate = "Today at " + string.Format("{0:t}", result.StartDate) + " or " + aHours.ToString() + " Hours away"; // ShortTime
              }
              else if (aDays == 1)  // Tomorrow
              {
                sDate = "Is Tomorrow at " + string.Format("{0:t}", result.StartDate);
              }
              else if (aDays > 1)  // x Days away
              {
                sDate = Convert.ToString(aDays) + " Days away at " + string.Format("{0:t}", result.StartDate);
              }
            }
            await DisplayAlert(result.Text, sDate, "OK");
          }
        }
      }
      _ToDay.Text = "Today: " + DateTime.Now.ToString("dddd  ddMMMyy  -  h:mmtt");
    }

    // ADD selected so go to the NoteEntryPage
    private async void OnAddClicked(object sender, EventArgs e)
    {
      // Navigate to the NoteEntryPage.
      await Shell.Current.GoToAsync(nameof(NoteEntryPage));
    }

    // Record selected so go to the NoteEntryPage passing the recNo.
    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.CurrentSelection != null)
      {
        // Navigate to the NoteEntryPage, passing the ID as a query parameter.
        Note note = (Note)e.CurrentSelection.FirstOrDefault();
        await Shell.Current.GoToAsync($"{nameof(NoteEntryPage)}?{nameof(NoteEntryPage.ItemId)}={note.ID}");
      }
    }

    // Keep the same day using days
    private DateTime AddDay(DateTime inDate, int offSet)
    {
      if (offSet == 1) { inDate = inDate.AddDays(1); }         // Daily
      else if (offSet == 2) { inDate = inDate.AddDays(7); }    // Weekly
      else if (offSet == 3) { inDate = inDate.AddDays(14); }   // Fortnightly
      else if (offSet == 4) { inDate = inDate.AddDays(21); }   // 3 Weekly
      else if (offSet == 5) { inDate = inDate.AddDays(28); }   // Monthly
      else if (offSet == 6) { inDate = inDate.AddDays(84); }   // 3 Monthly
      else if (offSet == 7) { inDate = inDate.AddDays(168); }  // 6 Monthly
      else if (offSet == 8) { inDate = inDate.AddDays(252); }  // 9 Monthly
      else if (offSet == 9) { inDate = inDate.AddDays(365); }; // Yearly  
      return inDate;
    }

    // Keep the same day using dates
    private DateTime AddDate(DateTime inDate, int offSet)
    {
      if (offSet == 1) { inDate = inDate.AddDays(1); }         // Daily
      else if (offSet == 2) { inDate = inDate.AddDays(7); }    // Weekly
      else if (offSet == 3) { inDate = inDate.AddDays(14); }   // Fortnightly
      else if (offSet == 4) { inDate = inDate.AddDays(21); }   // 3 Weekly
      else if (offSet == 5) { inDate = inDate.AddMonths(1); }  // Monthly
      else if (offSet == 6) { inDate = inDate.AddMonths(3); }  // 3 Monthly
      else if (offSet == 7) { inDate = inDate.AddMonths(6); }  // 6 Monthly
      else if (offSet == 8) { inDate = inDate.AddMonths(9); }  // 9 Monthly
      else if (offSet == 9) { inDate = inDate.AddYears(1); };  // Yearly  
      return inDate;
    }

    // Setup Notifications
    private async void SetupNotification(DateTime theDate, string outMessage, int _Hour, int _Min) // , int NotificationId
    {
      TimeSpan T1 = new TimeSpan(0, _Min, 0);  // Hour/ Min/ Sec
      DateTime alarm1 = theDate.Subtract(T1);    // Minutes;
      TimeSpan T2 = new TimeSpan(_Hour, 0, 0);  // Hour/ Min/ Sec
      DateTime alarm2 = theDate.Subtract(T2);    // Hours

      NotificationRequest notification = new NotificationRequest  // Minutes Alarm
      {
        NotificationId = notificationNumber++,
        Description = $"Dont't Forget: {outMessage} at {theDate}",
        Title = $"Appointment notification!",
        ReturningData = "Dummy data", // Returning data when tapped on notification.
        Schedule = { NotifyTime = alarm1 }
      };
      await NotificationCenter.Current.Show(notification);

      NotificationRequest notification2 = new NotificationRequest  // Hours Alarm
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
