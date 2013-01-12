using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneTasks.Resources;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Marketplace;


namespace PhoneTasks
{
    public partial class MainPage : PhoneApplicationPage
    {
        SavePhoneNumberTask savePhoneNumberTask = new SavePhoneNumberTask();
        SaveEmailAddressTask saveEmailAddressTask = new SaveEmailAddressTask();

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            
            savePhoneNumberTask.Completed += savePhoneTask_Completed;
            saveEmailAddressTask.Completed += saveEmailTask_Completed;

            App.RootFrame.Obscured += RootFrame_Obscured;
            App.RootFrame.Unobscured += RootFrame_Unobscured;
  
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LicenseInformation licenseInfo = new LicenseInformation();
            bool isTrial = licenseInfo.IsTrial();
            if (isTrial)
                homePageButton.Content = "Buy this application";
            else
                homePageButton.Content = "Marketplace home";

            base.OnNavigatedTo(e);
        }

        void RootFrame_Unobscured(object sender, System.EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Unobscured...");
        }

        void RootFrame_Obscured(object sender, ObscuredEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Obscured...");
        }

        private void SupportPhoneLink_Click(object sender, RoutedEventArgs e)
        {
            PhoneCallTask task = new PhoneCallTask()
            {
                PhoneNumber = (string)supportPhoneLink.Content,
                DisplayName = "WP8 In Action Customer Support"
            };
            task.Show();
        }

        private void SupportEmailLink_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask task = new EmailComposeTask()
            {
                To = (string)supportEmailLink.Content.ToString(),
                Subject = "WP8 in Action About Application",
                Body = "Support Issue Details:"
            };
            task.Show();
        }

        private void ShareSms_Click(object sender, RoutedEventArgs e)
        {
            SmsComposeTask task = new SmsComposeTask()
            {
                Body = "I like the WP8 in Action PhoneTasks Application, you should try it out!",
            };
            task.Show();
        }

        private void ShareLink_Click(object sender, RoutedEventArgs e)
        {
            ShareLinkTask task = new ShareLinkTask()
            {
                Title = "WP8 in Action",
                Message = "I like the WP8 in Action About Application, you should try it out!",
                LinkUri = new Uri("http://manning.com/perga")
            };
            task.Show();
        }

        private void ShareStatus_Click(object sender, RoutedEventArgs e)
        {
            ShareStatusTask task = new ShareStatusTask()
            {
                Status = "I like the WP8 in Action About Application, you should try it out!",
            };
            task.Show();
        }

        private void Review_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask task = new MarketplaceReviewTask();
            task.Show();
        }

        private void HomePage_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceDetailTask task = new MarketplaceDetailTask();
            task.Show();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceSearchTask task = new MarketplaceSearchTask()
            {
                SearchTerms = "Windows Phone 8 in Action",
                ContentType = MarketplaceContentType.Applications
            };
            task.Show();
        }

        private void BingSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchTask task = new SearchTask()
            {
                SearchQuery = "Windows Phone 8 in Action domain:manning.com"
            };
            task.Show();
        }

        private void SavePhone_Click(object sender, RoutedEventArgs e)
        {
            savePhoneNumberTask.PhoneNumber = (string)supportPhoneLink.Content;
            savePhoneNumberTask.Show();
        }

        private void savePhoneTask_Completed(object sender, TaskEventArgs e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                SavePhoneButton.Visibility = Visibility.Collapsed;
            }
        }

        private void SaveEmail_Click(object sender, RoutedEventArgs e)
        {
            saveEmailAddressTask.Email = (string)supportEmailLink.Content;
            saveEmailAddressTask.Show();
        }

        private void saveEmailTask_Completed(object sender, TaskEventArgs e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                saveEmailButton.Visibility = Visibility.Collapsed;
            }
        }


        #region Launchers and Choosers not covered in the book

        private void connectionSettings_Click(object sender, EventArgs e)
        {
            var task = new ConnectionSettingsTask
            {
                ConnectionSettingsType = ConnectionSettingsType.AirplaneMode,
            };
            task.Show();
        }

        private void marketplaceHub_Click(object sender, EventArgs e)
        {
            var task = new MarketplaceHubTask
            {
                ContentType = MarketplaceContentType.Applications
            };
            task.Show();
        }

        private void addressChooser_Click(object sender, EventArgs e)
        {
            // with a local task, the completed event will be called when returning from dormant,
            // but WILL NOT be called when returning from tombstone.
            AddressChooserTask task = new AddressChooserTask();
            task.Completed += (object sender2, AddressResult e2) =>
            {
                if (e2.Error == null && e2.TaskResult == TaskResult.OK)
                    System.Diagnostics.Debug.WriteLine("{0}: {1}", e2.DisplayName, e2.Address);
            };
            task.Show();
        }

        private void emailAddressChooser_Click(object sender, EventArgs e)
        {
            // with a local task, the completed event will be called when returning from dormant,
            // but WILL NOT be called when returning from tombstone.
            var task = new EmailAddressChooserTask();
            task.Completed += (object sender2, EmailResult e2) =>
            {
                if (e2.Error == null && e2.TaskResult == TaskResult.OK)
                    System.Diagnostics.Debug.WriteLine("{0}: {1}", e2.DisplayName, e2.Email);
            };
            task.Show();
        }

        private void phoneNumberChooser_Click(object sender, EventArgs e)
        {
            // with a local task, the completed event will be called when returning from dormant,
            // but WILL NOT be called when returning from tombstone.
            var task = new PhoneNumberChooserTask();
            task.Completed += (object sender2, PhoneNumberResult e2) =>
            {
                if (e2.Error == null && e2.TaskResult == TaskResult.OK)
                    System.Diagnostics.Debug.WriteLine("{0}: {1}", e2.DisplayName, e2.PhoneNumber);
            };
            task.Show();
        }

        private void saveContact_Click(object sender, EventArgs e)
        {
            var task = new SaveContactTask
            {
                FirstName = "Tim",
                LastName = "Jones",
                Company = "Magnatis",
                WorkEmail = "timjones@magnatis.com"
            };
            task.Show();
        }

        #endregion
    }
}