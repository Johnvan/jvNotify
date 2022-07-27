using jvNotify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;


namespace jvNotify.Views
{
  public partial class NotesPage : ContentPage
  {
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

      List<Note> textlist = await App.Database.GetNotesAsync();
      collectionView1.ItemsSource = from n in textlist
                                    where n.StartDate == DateTime.MinValue
                                    orderby n.StartDate ascending
                                    select n;

      List<Note> dates = await App.Database.GetNotesAsync();
      IOrderedEnumerable<Note> Temp = from n in dates
                                      where n.StartDate != DateTime.MinValue
                                      orderby n.StartDate ascending
                                      select n;
      foreach (Note result in Temp)
      {
        if ((result.StartDate > DateTime.MinValue) && (result.StartDate < DateTime.Now))
        {
          if (result.OnGo > 0)
          {
            if (result.TypeDate == 1)
            {
              result.StartDate = AddDay(result.StartDate, result.OnGo);
            }
            else if (result.TypeDate == 2)
            {
              result.StartDate = AddDate(result.StartDate, result.OnGo);
            }
            _ = await App.Database.SaveNoteAsync(result); // Save to Database
          }
          else
          {
            _ = await App.Database.DeleteNoteAsync(result);  // Delete old records
          }
        }
      }

      List<Note> reclist = await App.Database.GetNotesAsync();
      collectionView.ItemsSource = from n in reclist
                                   where n.StartDate > DateTime.MinValue
                                   orderby n.StartDate ascending
                                   select n;

      //List<Note> dates2 = await App.Database.GetNotesAsync();
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
                sDate = "Today at " + string.Format("{0:t}", result.StartDate) + " or " + aHours.ToString() + " Hours away";   // ShortTime
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
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
      // Navigate to the NoteEntryPage.
      await Shell.Current.GoToAsync(nameof(NoteEntryPage));
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.CurrentSelection != null)
      {
        // Navigate to the NoteEntryPage, passing the ID as a query parameter.
        Note note = (Note)e.CurrentSelection.FirstOrDefault();
        await Shell.Current.GoToAsync($"{nameof(NoteEntryPage)}?{nameof(NoteEntryPage.ItemId)}={note.ID.ToString()}");
      }
    }

    private DateTime AddDay(DateTime inDate, int offSet)  // Keeps the same day using days
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

    private DateTime AddDate(DateTime inDate, int offSet)  // Keeps the same day using dates
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
  }
}
