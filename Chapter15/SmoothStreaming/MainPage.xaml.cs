using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SmoothStreaming.Resources;
using Microsoft.Web.Media.SmoothStreaming;
using System.Windows.Media;

namespace SmoothStreaming
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

        private void mediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            stateTextBlock.Text = mediaElement.CurrentState.ToString();
            TrackInfo track = mediaElement.VideoPlaybackTrack;

            if (track != null)
            {
                string widthAttr;
                string heightAttr;
                track.Attributes.TryGetValue("Width", out widthAttr);
                track.Attributes.TryGetValue("Height", out heightAttr);
                sourceTextBlock.Text = string.Format("{0}x{1} {2}", widthAttr, heightAttr, track.Bitrate);
            }

            if (mediaElement.CurrentState == SmoothStreamingMediaElementState.Opening)
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

        private void mediaElement_ManifestReady(object sender, EventArgs e)
        {
            // find the tracks with the highest supported resolution
            const ulong maxSupportedHeight = 480;
            const ulong maxSupportedWidth = 720;

            // tracks are embedded in streams, which are embedded in segments
            foreach (SegmentInfo segment in mediaElement.ManifestInfo.Segments)
            {
                foreach (StreamInfo streamInfo in segment.AvailableStreams)
                {
                    // only worried about video in this example.
                    if (MediaStreamType.Video == streamInfo.Type)
                    {
                        List<TrackInfo> usableTracks = new List<TrackInfo>();
                        ulong bestHeight = 0;
                        ulong bestWidth = 0;

                        var availableTracks = streamInfo.AvailableTracks.Reverse();
                        foreach (TrackInfo track in availableTracks)
                        {
                            // get the height and width from of the track.
                            string widthAttr;
                            string heightAttr;
                            ulong width = 0;
                            ulong height = 0;

                            if (track.Attributes.TryGetValue("Width", out widthAttr))
                                ulong.TryParse(widthAttr, out width);

                            if (track.Attributes.TryGetValue("Height", out heightAttr))
                                ulong.TryParse(heightAttr, out height);

                            // ignore the track if the track is larger the the supported resolution,
                            // or if it is smaller that the current best resolution
                            // or if we didn't find a height or width
                            if (height > 0 && width > 0 &&
                                height <= maxSupportedHeight && width <= maxSupportedWidth)
                            {
                                // if the current track is larger than the existing track update the best resolution
                                if (height > bestHeight || width > bestWidth)
                                {
                                    bestHeight = height;
                                    bestWidth = width;
                                    usableTracks.Clear();
                                    usableTracks.Add(track);
                                }
                                else if (height == bestHeight && width == bestWidth)
                                {
                                    // the resoltuion is the same, add the track to the the list
                                    usableTracks.Add(track);
                                }
                            }
                        }
                        streamInfo.RestrictTracks(usableTracks);
                    }
                }
            }
        }

    }
}