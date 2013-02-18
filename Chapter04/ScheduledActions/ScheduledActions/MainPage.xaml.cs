using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using NotificationsUpdateAgent;
using Windows.System;

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
        
        private void DisplayScheduledNotifications()
        {
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            agentMessage.Text = ((App)Application.Current).AgentStatus;

            ScheduledAgent.UpdateDefaultTile();

            List<ScheduledAction> items = new List<ScheduledAction>();

            var notifications = ScheduledActionService
                .GetActions<ScheduledNotification>()
                .OrderBy((item) => item.BeginTime);

            foreach (ScheduledNotification notification in notifications)
            {
                ScheduledAction item = ScheduledActionService.Find(notification.Name);
                items.Add(item);
            }
            notificationList.ItemsSource = items;
        }

        private void AddReminder_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ReminderPage.xaml", UriKind.Relative));
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            Launcher.LaunchUriAsync(new Uri("ms-settings-lock:"));
        }

        private void NotificationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("selection changed");
            ScheduledNotification notification =
                notificationList.SelectedItem as ScheduledNotification;
            if (notification != null)
            {
                NavigationService.Navigate(new Uri("/ReminderPage.xaml?name=" + notification.Name, UriKind.Relative));
            }
        }

        
    }
}