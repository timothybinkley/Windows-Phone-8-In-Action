using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MediaPlayback.Resources;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Media;
using Windows.Storage;
using Microsoft.Phone.Tasks;


namespace MediaPlayback
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

      private void LaunchVideoFromInstall_Click(object sender, EventArgs e)
      {
         var task = new MediaPlayerLauncher();
         task.Location = MediaLocationType.Install;
         task.Media = new Uri("Assets/sample.wmv", UriKind.Relative);
         task.Show();
      }

      private void LaunchVideoFromStorage_Click(object sender, EventArgs e)
      {
         var task = new MediaPlayerLauncher();
         task.Media = new Uri("sample.wmv", UriKind.Relative);
         task.Show();
      }

      private void LaunchVideoFromWeb_Click(object sender, EventArgs e)
      {
         var task = new MediaPlayerLauncher();
         task.Location = MediaLocationType.None;
         task.Media = new Uri("http://www.windowsphoneinaction.com/sample.wmv");
         task.Show();
      }

      private void VideoFromInstall_Click(object sender, EventArgs e)
      {
         mediaElement.Source = new Uri("Assets/sample2.wmv", UriKind.Relative);
         sourceTextBlock.Text = "video from install";
      }

      private void VideoFromStorage_Click(object sender, EventArgs e)
      {
         mediaElement.Source = null;
         using (var store = IsolatedStorageFile.GetUserStoreForApplication())
         {
            if (store.FileExists("sample.wmv"))
            {
               var fileStream = new IsolatedStorageFileStream("sample.wmv", FileMode.Open, store);
               mediaElement.SetSource(fileStream);
               sourceTextBlock.Text = "video from storage";
            }
            sourceTextBlock.Text = "video from storage";
         }
      }

      private void VideoFromWeb_Click(object sender, EventArgs e)
      {
         mediaElement.Source = new Uri("http://www.windowsphoneinaction.com/sample.wmv");
         sourceTextBlock.Text = "video from web";
      }

      private void Play_Click(object sender, EventArgs e)
      {
         mediaElement.Play();
      }

      private void Pause_Click(object sender, EventArgs e)
      {
         mediaElement.Pause();
      }

      private void Stop_Click(object sender, EventArgs e)
      {
         mediaElement.Stop();
      }

      private void Mute_Click(object sender, EventArgs e)
      {
         mediaElement.IsMuted = !mediaElement.IsMuted;
         mutedTextBlock.Text = mediaElement.IsMuted ? "muted" : string.Empty;
      }

      private void mediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
      {
         stateTextBlock.Text = mediaElement.CurrentState.ToString();
         if (mediaElement.CurrentState == MediaElementState.Opening)
            mediaProgress.IsVisible = true;
         else
            mediaProgress.IsVisible = false;
      }

      private void mediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
      {
         MessageBox.Show(e.ErrorException.Message, "Media Failure", MessageBoxButton.OK);
      }

      private void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)
      {

      }


   }
}