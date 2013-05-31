using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MapsTasks.Resources;
using Microsoft.Phone.Tasks;
using Windows.System;

namespace MapsTasks
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

      private void mapTask_Click(object sender, EventArgs e)
      {
         if (string.IsNullOrEmpty(departureTerm.Text))
         {
            MessageBox.Show("Please enter a start location.");
            return;
         }

         var task = new MapsTask
         {
            // Center = ...
            SearchTerm = departureTerm.Text,
            ZoomLevel = 15,
         };
         task.Show();
      }

      private void directionTask_Click(object sender, EventArgs e)
      {
         LabeledMapLocation start = null;
         LabeledMapLocation end = null;

         if (!string.IsNullOrEmpty(departureTerm.Text))
            start = new LabeledMapLocation { Label = departureTerm.Text };

         if (!string.IsNullOrEmpty(destinationTerm.Text))
            end = new LabeledMapLocation { Label = destinationTerm.Text };

         if (start == null && end == null)
         {
            MessageBox.Show("Please enter start and/or end locations.");
            return;
         }

         var task = new MapsDirectionsTask { Start = start, End = end };
         task.Show();
      }

   }
}