//using Microsoft.Devices.Sensors;
using Microsoft.Phone.Controls;
using System;
using System.IO;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;

namespace Sensors
{
   public partial class MainPage : PhoneApplicationPage
   {
      DispatcherTimer timer;
      Accelerometer accelSensor;
      Compass compassSensor;
      Gyrometer gyroSensor;
      Inclinometer inclineSensor;
      OrientationSensor orientationSensor;

      // Constructor
      public MainPage()
      {
         InitializeComponent();

         // Sample code to localize the ApplicationBar
         //BuildLocalizedApplicationBar();

         timer = new DispatcherTimer();
         timer.Tick += timer_Tick;
         timer.Interval = TimeSpan.FromMilliseconds(66);

         start();
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

      private async void start()
      {
         if (!timer.IsEnabled)
         {
            string runningMessage = "Reading: ";

            accelSensor = Accelerometer.GetDefault();
            if (accelSensor != null)
            {
               accelSensor.ReportInterval = 66;
               runningMessage += "Accelerometer ";
            }

            // while not shown in the chapter, get the current location so that 
            // true heading is more accurate.
            Geolocator locator = new Geolocator();
            await locator.GetGeopositionAsync();

            compassSensor = Compass.GetDefault();
            if (compassSensor != null)
            {
               compassSensor.ReportInterval = 66;
               runningMessage += "Compass ";
            }

            try
            {
               gyroSensor = Gyrometer.GetDefault();
            }
            catch (FileNotFoundException) { }

            if (gyroSensor != null)
            {
               gyroSensor.ReportInterval = 66;
               runningMessage += "Gyroscope ";
            }

            inclineSensor = Inclinometer.GetDefault();
            if (inclineSensor != null)
            {
               inclineSensor.ReportInterval = 66;
               runningMessage += "Inclinometer ";
            }

            orientationSensor = OrientationSensor.GetDefault();
            if (orientationSensor != null)
            {
               orientationSensor.ReportInterval = 66;
               runningMessage += "Orientation ";
            }

            timer.Start();
            messageBlock.Text = runningMessage;
         }
      }

      void timer_Tick(object sender, EventArgs e)
      {
         ReadAccelerometerData();
         ReadCompassData();
         ReadGyrometerData();
         ReadInclinometerData();
         ReadOrientationData();
      }

      void ReadAccelerometerData()
      {
         if (accelSensor != null)
         {
            AccelerometerReading reading = accelSensor.GetCurrentReading();
            if (reading != null)
            {
               accelX.Value = reading.AccelerationX;
               accelY.Value = reading.AccelerationY;
               accelZ.Value = reading.AccelerationZ;
            }
         }
      }

      void ReadCompassData()
      {
         if (compassSensor != null)
         {
            CompassReading reading = compassSensor.GetCurrentReading();
            if (reading != null)
            {
               heading.Text = string.Format(
                  "Magnetic Heading={0:F0}° True Heading={1:F0}°",  
                 reading.HeadingMagneticNorth, reading.HeadingTrueNorth);
            }
         }
      }

      void ReadGyrometerData()
      {
         if (gyroSensor != null)
         {
            GyrometerReading reading = gyroSensor.GetCurrentReading();
            if (reading != null)
            {
               gyroX.Value = reading.AngularVelocityX;
               gyroY.Value = reading.AngularVelocityY;
               gyroZ.Value = reading.AngularVelocityZ;
            }
         }
      }

      void ReadInclinometerData()
      {
         if (inclineSensor != null)
         {
            InclinometerReading reading = inclineSensor.GetCurrentReading();
            if (reading != null)
            {
               inclineX.Value = reading.PitchDegrees;
               inclineY.Value = reading.RollDegrees;
               inclineZ.Value = reading.YawDegrees;
            }
         }
      }

      static readonly Matrix3D pointMatrix = new Matrix3D(
                  0, 0, 0, 0,
                  0, 0, 0, 0,
                  0, 0, 0, 0,
                  0, 10.0, 0, 1);

      void ReadOrientationData()
      {
         if (orientationSensor != null)
         {
            OrientationSensorReading reading = orientationSensor.GetCurrentReading();
            if (reading != null)
            {
               SensorRotationMatrix srm = reading.RotationMatrix;
               Matrix3D rotationMatrix = new Matrix3D(
                  srm.M11, srm.M12, srm.M13, 0,
                  srm.M21, srm.M22, srm.M23, 0,
                  srm.M31, srm.M32, srm.M33, 0,
                  0, 0, 0, 0);

               Matrix3D bodySpaceMatrix = pointMatrix * rotationMatrix;
               point.Text = string.Format("Transform of (0.0, 10.0, 0.0) = ({0:F1}, {1:F1}, {2:F1})",
                   bodySpaceMatrix.OffsetX, bodySpaceMatrix.OffsetY, bodySpaceMatrix.OffsetZ);
            }
         }
      }
   }
}