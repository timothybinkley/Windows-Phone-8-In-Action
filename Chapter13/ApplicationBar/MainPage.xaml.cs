using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ApplicationBar.Resources;

namespace ApplicationBar
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
            InitializeApplicationBar();
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
              
        private void InitializeApplicationBar()
        {
            button1 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            menuItem1 = (ApplicationBarMenuItem)ApplicationBar.MenuItems[0];
        }

        private void item_Clicked(object sender, EventArgs e)
        {
            var button = sender as ApplicationBarIconButton;
            if (button != null)
            {
                MessageBox.Show(button.Text, "Button Clicked", MessageBoxButton.OK);
            }
            else
            {
                var menuItem = sender as ApplicationBarMenuItem;
                MessageBox.Show(menuItem.Text, "Menu Item Clicked", MessageBoxButton.OK);
            }
        }

        private void appBarVisible_Clicked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            ApplicationBar.IsVisible = checkBox.IsChecked.Value;
        }

        private void appBarMode_Clicked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            if (checkBox.IsChecked.Value)
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
            else
                ApplicationBar.Mode = ApplicationBarMode.Default;
            LayoutRoot.UpdateLayout();
        }

        private void appBarOpacity_Clicked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            if (checkBox.IsChecked.Value)
                ApplicationBar.Opacity = 0.5;
            else
                ApplicationBar.Opacity = 1.0;
        }

        private void appBarEnabled_Clicked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            ApplicationBar.IsMenuEnabled = checkBox.IsChecked.Value;
        }

        private void button1Enabled_Clicked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            button1.IsEnabled = checkBox.IsChecked.Value;
        }

        private void menuItem1Enabled_Clicked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            menuItem1.IsEnabled = checkBox.IsChecked.Value;
        }

        private void button1Show_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            if (checkBox.IsChecked.Value)
                ApplicationBar.Buttons.Insert(0, button1);
            else
                ApplicationBar.Buttons.Remove(button1);
        }

        private void menuItem1Show_Clicked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            if (checkBox.IsChecked.Value)
                ApplicationBar.MenuItems.Insert(0, menuItem1);
            else
                ApplicationBar.MenuItems.Remove(menuItem1);
        }

        private void button1_Clicked(object sender, EventArgs e)
        {
            if (button1.Text == "alpha")
            {
                button1.Text = "omega";
                button1.IconUri = new Uri("/Assets/AppBar/omega.png", UriKind.Relative);
            }
            else
            {
                button1.Text = "alpha";
                button1.IconUri = new Uri("/Assets/AppBar/alpha.png", UriKind.Relative);
            }
        }

        private void ApplicationBar_StateChanged(object sender, ApplicationBarStateChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("IsMenuVisible: " + e.IsMenuVisible);
        }

        private void contextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            MessageBox.Show(menuItem.Name, "Context Menu Item Clicked",
                MessageBoxButton.OK);

        }

    }
}