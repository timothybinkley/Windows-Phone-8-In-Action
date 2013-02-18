using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

namespace ScheduledActions
{
    public partial class ReminderPage : PhoneApplicationPage
    {
        Reminder reminder;

        public ReminderPage()
        {
            InitializeComponent();
            RecurrenceInterval[] values = (RecurrenceInterval[])Enum.GetValues(typeof(RecurrenceInterval));
            listPicker.ItemsSource = values;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                if (NavigationContext.QueryString.ContainsKey("name"))
                {
                    string name = NavigationContext.QueryString["name"];
                    reminder = (Reminder)ScheduledActionService.Find(name);
                }
                else
                {
                    reminder = new Reminder(Guid.NewGuid().ToString());
                    reminder.Title = "Reminder";
                    reminder.BeginTime = DateTime.Now;
                    reminder.NavigationUri = new Uri(
                            "/MainPage.xaml?reminder=" + reminder.Name, UriKind.Relative);
                }
                this.DataContext = reminder;
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            TextBox textInput = FocusManager.GetFocusedElement() as TextBox;
            if (textInput != null)
                textInput.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            if (string.IsNullOrWhiteSpace(reminder.Title))
                reminder.Title = "(no title)";

            if (string.IsNullOrWhiteSpace(reminder.Content))
                reminder.Content = "(no description)";

            DateTime date = datePicker.Value.Value;
            DateTime time = timePicker.Value.Value;
            reminder.BeginTime = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);

            try
            {
                if (ScheduledActionService.Find(reminder.Name) == null)
                    ScheduledActionService.Add(reminder);
                else
                    ScheduledActionService.Replace(reminder);
                NavigationService.GoBack();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (ScheduledActionService.Find(reminder.Name) != null)
            {
                ScheduledActionService.Remove(reminder.Name);
            }
            NavigationService.GoBack();
        }
    }
}