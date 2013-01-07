using Microsoft.Phone.Controls;
using Microsoft.Phone.Marketplace;
using System;
using System.Windows.Media;
using System.Windows.Navigation;

namespace HelloWorld
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

            globeBrush = (SolidColorBrush)ContentPanel.Resources["GlobeBrush"];
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
       
        Color[] colors = new Color[] { Colors.Red, Colors.Orange, Colors.Yellow, Colors.Green, Colors.Blue, Colors.Purple };
        int colorIndex = 0;
        SolidColorBrush globeBrush;

        private void Canvas_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            colorIndex++;
            if (colorIndex >= colors.Length)
                colorIndex = 0;
            globeBrush.Color = colors[colorIndex];
        }

        private void Canvas_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            globeBrush.Color = (Color)App.Current.Resources["PhoneAccentColor"];
        }

        private void nameInput_ActionIconTapped(object sender, EventArgs e)
        {
            NavigationService.Navigate(GreetingPage.BuildNavigationUri(nameInput.Text));
        }

    }
}