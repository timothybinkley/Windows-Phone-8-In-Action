using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ScheduledActions.Resources;
using Microsoft.Phone.Scheduler;

namespace ScheduledActions
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        private void AddReminder_Click(object sender, EventArgs e)
        {
            string reminderName = string.Format("Reminder {0}", Guid.NewGuid());

            Reminder reminder = new Reminder(reminderName);
            reminder.BeginTime = DateTime.Now.AddMinutes(1);
            reminder.RecurrenceType = RecurrenceInterval.Daily;
            reminder.Content = "This is a WP8 in Action Reminder";
            reminder.Title = "Reminders in action";
            reminder.NavigationUri = new Uri(
                "/MainPage.xaml?reminder=" + reminderName, UriKind.Relative);

            ScheduledActionService.Add(reminder);
            DisplayScheduledNotifications();
        }

        private void RescheduleNotification_Click(object sender, EventArgs e)
        {
            var notification =
                notificationList.SelectedItem as ScheduledNotification;
            if (notification != null)
            {
                notification.BeginTime = DateTime.Now.AddMinutes(1);
                notification.Content = "*" + notification.Content;

                ScheduledActionService.Replace(notification);
                DisplayScheduledNotifications();
            }
        }


        private void RemoveNotification_Click(object sender, EventArgs e)
        {
            var notification = notificationList.SelectedItem
                as ScheduledNotification;
            if (notification != null)
            {
                ScheduledActionService.Remove(notification.Name);
                DisplayScheduledNotifications();
            }
        }

        protected void DisplayScheduledNotifications()
        {
            var items = new List<ScheduledAction>();
            var notifications = ScheduledActionService.
                GetActions<ScheduledNotification>();
            foreach (var notification in notifications)
            {
                var item = ScheduledActionService.Find(notification.Name);
                items.Add(item);
            }
            notificationList.ItemsSource = items;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            agentMessage.Text = ((App)Application.Current).AgentStatus;
            DisplayScheduledNotifications();
            base.OnNavigatedTo(e);
        }
    }
}