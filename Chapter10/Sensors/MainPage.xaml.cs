//using Microsoft.Devices.Sensors;
using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Windows.Devices.Sensors;

namespace Sensors
{
   public partial class MainPage : PhoneApplicationPage
   {
      DispatcherTimer timer;
      Accelerometer accelSensor;
      Inclinometer inclineSensor;
      Compass compassSensor;
      Gyrometer gyroSensor;
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

         accelSensor = Accelerometer.GetDefault();
         if (accelSensor != null)
            accelSensor.ReportInterval = 66;

         inclineSensor = Inclinometer.GetDefault();
         if (inclineSensor != null)
            inclineSensor.ReportInterval = 66;

         compassSensor = Compass.GetDefault();
         if(compassSensor != null)
            compassSensor.ReportInterval = 66;

         gyroSensor = Gyrometer.GetDefault();
         if(gyroSensor != null)
            gyroSensor.ReportInterval = 66;
         
         orientationSensor = OrientationSensor.GetDefault();
         if (orientationSensor != null)
            orientationSensor.ReportInterval = 66;

         start_Click(null, null);
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

      private void start_Click(object sender, EventArgs e)
      {
         if (!timer.IsEnabled)
         {
            string runningMessage = "Reading: ";
            //if (Accelerometer.IsSupported)
            if(accelSensor != null)
            {
               //accelSensor.Start();
               runningMessage += "Accelerometer ";
            }
            
            if (inclineSensor != null)
            {
               //accelSensor.Start();
               runningMessage += "Inclinometer ";
            }

            //if (Compass.IsSupported)
            if(compassSensor != null)
            {
               //compassSensor.Start();
               runningMessage += "Compass ";
            }

            //if (Gyroscope.IsSupported)
            if(gyroSensor != null)
            {
               //gyroSensor.Start();
               runningMessage += "Gyroscope ";
            }

            if (gyroSensor != null)
            {
               runningMessage += "Orientation ";
            }

            timer.Start();
            messageBlock.Text = runningMessage;
         }
      }

      void timer_Tick(object sender, EventArgs e)
      {
         ReadAccelerometerData();
         ReadInclinometerData();
         ReadCompassData();
         ReadGyrometerData();
         ReadOrientationData();
      }

      private void ReadAccelerometerData()
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

      private void ReadInclinometerData()
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

      void ReadCompassData()
      {
         if(compassSensor != null)
         {
            CompassReading reading = compassSensor.GetCurrentReading();
            heading.Text = string.Format("Compass Heading True={0:F} Magnetic={1:F} degrees", reading.HeadingTrueNorth, reading.HeadingMagneticNorth);
         }
      }

      void ReadGyrometerData()
      {
         if(gyroSensor != null)
         {
            GyrometerReading reading = gyroSensor.GetCurrentReading();
            gyroX.Value = reading.AngularVelocityX;
            gyroY.Value = reading.AngularVelocityY;
            gyroZ.Value = reading.AngularVelocityZ;
         }
      }

      void ReadOrientationData()
      {
         OrientationSensorReading reading = orientationSensor.GetCurrentReading();
         SensorRotationMatrix srm = reading.RotationMatrix;
         Matrix3D rotationMatrix = new Matrix3D(
            srm.M11, srm.M12, srm.M13, 0,
            srm.M21, srm.M22, srm.M23, 0,
            srm.M31, srm.M32, srm.M33, 0,
            0, 0, 0, 0);

         Matrix3D pointMatrix = new Matrix3D(
            0,0,0,0,
            0,0,0,0,
            0,0,0,0,
            0,10.0,0,1);

         Matrix3D bodySpaceMatrix = pointMatrix * rotationMatrix;
         //Vector3 worldSpacePoint = new Vector3(0.0f, 10.0f, 0.0f);
         //Vector3 bodySpacePoint = Vector3.Transform(worldSpacePoint, rotationMatrix);
         point.Text = string.Format("Transform of (0.0, 10.0, 0.0) = ({0:F1}, {1:F1}, {2:F1})",
            //bodySpacePoint.X, bodySpacePoint.Y, bodySpacePoint.Z);
             bodySpaceMatrix.OffsetX, bodySpaceMatrix.OffsetY, bodySpaceMatrix.OffsetZ);

      }
   }
}