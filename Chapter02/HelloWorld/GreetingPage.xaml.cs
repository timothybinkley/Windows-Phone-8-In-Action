using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;

namespace HelloWorld
{
    public partial class GreetingPage : PhoneApplicationPage
    {
        public GreetingPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            helloMessage.Text = this.NavigationContext.QueryString["name"];
            base.OnNavigatedTo(e);
        }

        private void Page_BackKeyPress(object sender, CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Press OK to return to the previous page.",
                "WP8 in Action", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel)
                e.Cancel = true;
        }

        private void navigateBackButton_Click(object sender, EventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            string message = string.Format("Hello {0}!", helloMessage.Text);
            Clipboard.SetText(message);
        }

        private void pinButton_Click(object sender, EventArgs e)
        {
            var tileData = new FlipTileData
            {
                SmallBackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileSmall.png", UriKind.Relative),
                BackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileMedium.png", UriKind.Relative),
                WideBackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileLarge.png", UriKind.Relative),
                BackTitle = string.Format("Hello {0}!", helloMessage.Text),
                BackContent = "Windows Phone 8 in Action",
                WideBackContent = "Windows Phone 8 in Action. Written by Binkley-Jones, Perga, Sync, and Benoit.",
            };
            ShellTile.Create(BuildNavigationUri(helloMessage.Text), tileData, true);
        }

        public static Uri BuildNavigationUri(string name)
        {
            return new Uri("/GreetingPage.xaml?name=" + name, UriKind.Relative);
        }

    }
}