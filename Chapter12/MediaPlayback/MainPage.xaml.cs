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

        private void PlayClicked(object sender, EventArgs e)
        {
            mediaElement.Play();
        }

        private void PauseClicked(object sender, EventArgs e)
        {
            mediaElement.Pause();
        }

        private void StopClicked(object sender, EventArgs e)
        {
            mediaElement.Stop();
        }

        private void MuteClicked(object sender, EventArgs e)
        {
            mediaElement.IsMuted = !mediaElement.IsMuted;
            mutedTextBlock.Text = mediaElement.IsMuted ? "muted" : string.Empty;
        }

        private void VideoFromXapClicked(object sender, EventArgs e)
        {
            mediaElement.Source = new Uri("Media/sample.wmv", UriKind.Relative);
            sourceTextBlock.Text = "video from xap";
        }

        private void VideoFromStorageClicked(object sender, EventArgs e)
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
            }
        }

        private void VideoFromWebClicked(object sender, EventArgs e)
        {
            mediaElement.Source = new Uri(
                "http://www.wp7inaction.com/sample.wmv",
                UriKind.Absolute);
            sourceTextBlock.Text = "video from web";
        }

        private void CustomVideoClicked(object sender, EventArgs e)
        {
            mediaElement.Source = null;
            MediaStreamSource source = new VideoMediaStreamSource(
                (int)mediaElement.ActualWidth,
                (int)mediaElement.ActualHeight);
            mediaElement.SetSource(source);
            sourceTextBlock.Text = "custom video";
        }

        private void CustomAudioClicked(object sender, EventArgs e)
        {
            mediaElement.Source = null;
            MediaStreamSource source = new AudioMediaStreamSource(440);
            mediaElement.SetSource(source);
            sourceTextBlock.Text = "custom audio";
        }

        private void mediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            stateTextBlock.Text = mediaElement.CurrentState.ToString();
            if (mediaElement.CurrentState == MediaElementState.Opening)
                //mediaProgress.Visibility = Visibility.Visible;
                mediaProgress.IsVisible = true;
            else
                //mediaProgress.Visibility = Visibility.Collapsed;
                mediaProgress.IsVisible = false;
        }

        private void mediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show(e.ErrorException.Message, "Media Failure", MessageBoxButton.OK);
        }
    }
}