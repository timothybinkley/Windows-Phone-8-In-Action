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
            StandardTileData tileData = new StandardTileData
            {
                BackgroundImage = new Uri("Background.png", UriKind.Relative),
                Title = string.Format("Hello {0}!", helloMessage.Text),
            };
            ShellTile.Create(BuildNavigationUri(helloMessage.Text), tileData);
        }

        public static Uri BuildNavigationUri(string name)
        {
            return new Uri("/GreetingPage.xaml?name=" + name, UriKind.Relative);
        }

    }
}