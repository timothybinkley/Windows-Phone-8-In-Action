using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ringtones.Resources;
using Microsoft.Phone.Tasks;

namespace Ringtones
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

        private void SaveRingtoneButton_Click(object sender, EventArgs e)
        {
            SaveRingtoneTask task = new SaveRingtoneTask()
            {
                DisplayName = "ringtones in action",
                Source = new Uri("appdata:/mytone.wma"),
                IsShareable = true,
            };
            task.Completed += saveRingtoneTask_Completed;
            task.Show();
        }

        void saveRingtoneTask_Completed(object sender, TaskEventArgs e)
        {
            //string chooserResult = "";
            if (e.Error != null)
                chooserResult.Text = e.Error.Message;
            else if (e.TaskResult == TaskResult.Cancel)
                chooserResult.Text = "user canceled";
            else if (e.TaskResult == TaskResult.None)
                chooserResult.Text = "no result";
            else if (e.TaskResult == TaskResult.OK)
                chooserResult.Text = "ok";
            System.Diagnostics.Debug.WriteLine(chooserResult.Text);
        }
    }
}