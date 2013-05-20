using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace HTML5App
{
   public partial class MainPage : PhoneApplicationPage
   {
      // Url of Home page
      private string MainUri = "/Html/index.html";

      // Constructor
      public MainPage()
      {
         InitializeComponent();
      }

      private void Browser_Loaded(object sender, RoutedEventArgs e)
      {
         // Add your URL here
         Browser.IsScriptEnabled = true;
         Browser.Navigate(new Uri(MainUri, UriKind.Relative));
        
      }

      // Navigates back in the web browser's navigation stack, not the applications.
      private void BackApplicationBar_Click(object sender, EventArgs e)
      {
         Browser.GoBack();
      }

      // Navigates forward in the web browser's navigation stack, not the applications.
      private void ForwardApplicationBar_Click(object sender, EventArgs e)
      {
         Browser.GoForward();
      }

      // Navigates to the initial "home" page.
      private void HomeMenuItem_Click(object sender, EventArgs e)
      {
         Browser.Navigate(new Uri(MainUri, UriKind.Relative));
      }

      // Handle navigation failures.
      private void Browser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
      {
         MessageBox.Show("Navigation to this page failed, check your internet connection");
      }

      private void about_Click(object sender, EventArgs e)
      {
         var task = new WebBrowserTask
         {
            Uri = new Uri("http://www.manning.com/binkley")
         };
         task.Show();
      }

      private void Browser_ScriptNotify(object sender, NotifyEventArgs e)
      {
         if (e.Value == "chooseAddress")
         {
            var task = new AddressChooserTask();
            task.Completed += task_Completed;
            task.Show();
         }
      }

      private void task_Completed(object sender, AddressResult e)
      {
         string message;
         if (e.Error != null || e.TaskResult != TaskResult.OK)
            message = "No address chosen";
         else
            message = e.Address.Replace("\r\n", ",");
         Browser.InvokeScript("addressChooserCompleted", message);
      }

   }
}