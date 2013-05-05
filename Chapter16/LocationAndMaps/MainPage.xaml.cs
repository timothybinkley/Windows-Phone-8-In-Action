using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LocationAndMaps.Resources;
using Windows.Devices.Geolocation;
using System.Text;
using System.Device.Location;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using System.Windows.Media;

namespace LocationAndMaps
{
   public partial class MainPage : PhoneApplicationPage
   {
      UserLocationMarker marker;
      int pinNumber = 0;

      Geolocator service;
      GeoCoordinate previous = new GeoCoordinate();
      DateTime previousTime = DateTime.Now;

      MapPolyline routeLine;

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

      private async void mapControl_Loaded(object sender, RoutedEventArgs e)
      {
         status.Text = "querying for current location...";

         Geolocator locator = new Geolocator();
         Geoposition geoPosition = await locator.GetGeopositionAsync();
         GeoCoordinate coordinate = geoPosition.Coordinate.ToGeoCoordinate();
         mapControl.Center = coordinate;
         mapControl.ZoomLevel = 10;

         position.Text = string.Format("Latitude: {0}\nLongitude: {1}\n",
             FormatCoordinate(coordinate.Latitude, 'N', 'S'),
             FormatCoordinate(coordinate.Longitude, 'E', 'W'));

         marker = new UserLocationMarker();
         marker.GeoCoordinate = coordinate;
         MapExtensions.GetChildren(mapControl).Add(marker);

         status.Text += "complete";
      }

      private string FormatCoordinate(double coordinate, char positive, char negative)
      {
         char direction = coordinate >= 0 ? positive : negative;
         coordinate = Math.Abs(coordinate);
         double degrees = Math.Floor(coordinate);
         double minutes = Math.Floor((coordinate - degrees) * 60.0D);
         double seconds = (((coordinate - degrees) * 60.0D) - minutes) * 60.0D;
         string result = string.Format("{0}{1:F0}° {2:F0}' {3:F1}\"",
             direction, degrees, minutes, seconds);
         return result;
      }

      private async void mapControl_Hold(object sender, System.Windows.Input.GestureEventArgs e)
      {
         status.Text = "querying for address...";

         var point = e.GetPosition(mapControl);
         var coordinate = mapControl.ConvertViewportPointToGeoCoordinate(point);

         var pushpin = new Pushpin
         {
            GeoCoordinate = coordinate,
            Content = ++pinNumber,
         };
         MapExtensions.GetChildren(mapControl).Add(pushpin);

         position.Text = string.Format("Latitude: {0}\nLongitude: {1}\n",
             FormatCoordinate(coordinate.Latitude, 'N', 'S'),
             FormatCoordinate(coordinate.Longitude, 'E', 'W'));

         ReverseGeocodeQuery query = new ReverseGeocodeQuery { GeoCoordinate = coordinate };
         IList<MapLocation> results = await query.GetMapLocationsAsync();
         position.Text += string.Format("{0} locations found.\n", results.Count);
         MapLocation location = results.FirstOrDefault();
         if (location != null)
         {
            position.Text += FormatAddress(location.Information.Address);
         }
         status.Text += "complete";
      }

      private string FormatAddress(MapAddress address)
      {
         StringBuilder b = new StringBuilder();

         if (!string.IsNullOrWhiteSpace(address.HouseNumber))
            b.AppendFormat("{0} ", address.HouseNumber);

         if (!string.IsNullOrWhiteSpace(address.Street))
            b.AppendFormat("{0}\n", address.Street);

         if (!string.IsNullOrWhiteSpace(address.City))
            b.AppendFormat("{0}, ", address.City);

         b.AppendFormat("{0}  {1}", address.State, address.PostalCode);

         return b.ToString();
      }

      #region continuous location tracking

      private void start_Click(object sender, EventArgs e)
      {
         ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
         ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true;

         service = new Geolocator();
         service.DesiredAccuracy = PositionAccuracy.High;
         service.MovementThreshold = 1.0;
         service.PositionChanged += service_PositionChanged;
         service.StatusChanged += service_StatusChanged;

         position.Text = string.Empty;
         status.Text = service.LocationStatus.ToString();
         mapControl.Pitch = 45.0;

         var startPin = new Pushpin
         {
            GeoCoordinate = marker.GeoCoordinate,
            Content = "Start"
         };
         MapExtensions.GetChildren(mapControl).Add(startPin);

         routeLine = new MapPolyline
         {
            StrokeColor = (Color)Resources["PhoneAccentColor"],
            StrokeThickness = (double)Resources["PhoneStrokeThickness"]
         };
         routeLine.Path.Add(marker.GeoCoordinate);
         mapControl.MapElements.Add(routeLine);
      }

      private void stop_Click(object sender, EventArgs e)
      {
         ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true;
         ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false;
         service.PositionChanged -= service_PositionChanged;
         service.StatusChanged -= service_StatusChanged;
         service = null;
         status.Text += "\nContinuous tracking has been stopped.";
         mapControl.Pitch = 0;
         previous = new GeoCoordinate();
      }

      private void service_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
      {
         Dispatcher.BeginInvoke(() =>
         {
            status.Text = string.Format("Status: {0}  Desired accuracy: {1}",
                 args.Status, service.DesiredAccuracy);  // desired accuracy in meters?
         });
      }

      private void service_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
      {
         System.Diagnostics.Debug.WriteLine("position changed " + DateTime.Now);
         GeoCoordinate location = args.Position.Coordinate.ToGeoCoordinate();
         UpdatePositionText(location);
         UpdateMap(location);
         previous = location;
         previousTime = DateTime.Now;
      }

      private void UpdatePositionText(GeoCoordinate location)
      {
         var b = new StringBuilder();
         b.AppendFormat("Latitude: {0} ± {1:F0} meters\n",
             FormatCoordinate(location.Latitude, 'N', 'S'),
             location.HorizontalAccuracy);
         b.AppendFormat("Longitude: {0} ± {1:F0} meters\n",
             FormatCoordinate(location.Longitude, 'E', 'W'),
             location.HorizontalAccuracy);

         b.AppendFormat("Altitude: {0:F0} ± {1:F0} meters\n",
             location.Altitude, location.VerticalAccuracy);
         b.AppendFormat("Heading: {0:F0} degrees from true north\n", location.Course);
         b.AppendFormat("Speed: {0:F0} meters/second\n", location.Speed);
         double distance = Double.NaN;
         if (!previous.IsUnknown)
            distance = location.GetDistanceTo(previous);
         b.AppendFormat("Distance: {0:F0} meters from previous reading\n",
             distance);
         b.AppendFormat("Time: {0} seconds from previous reading\n", (DateTime.Now - previousTime).Seconds);
         Dispatcher.BeginInvoke(() =>
         {
            position.Text = b.ToString();
         });
      }

      private void UpdateMap(GeoCoordinate location)
      {
         Dispatcher.BeginInvoke(() =>
         {
            marker.GeoCoordinate = location;
            routeLine.Path.Add(location);
            mapControl.SetView(LocationRectangle.CreateBoundingRectangle(routeLine.Path));
         });
      }

      #endregion
   }
}