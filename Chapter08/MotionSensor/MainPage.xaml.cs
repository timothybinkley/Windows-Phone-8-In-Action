using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MotionSensor.Resources;
using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;

namespace MotionSensor
{
    public partial class MainPage : PhoneApplicationPage
    {
        Motion sensor;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

            if (Motion.IsSupported)
            {
                sensor = new Motion();
                sensor.TimeBetweenUpdates = TimeSpan.FromMilliseconds(66);
                sensor.CurrentValueChanged += sensor_CurrentValueChanged;
                sensor.Calibrate += sensor_Calibrate;
            }
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
        
        void sensor_CurrentValueChanged(object sender, SensorReadingEventArgs<MotionReading> e)
        {
            MotionReading reading = e.SensorReading;
            Dispatcher.BeginInvoke(() =>
            {
                Vector3 acceleration = reading.DeviceAcceleration;
                // height of control = 400; height of postive bar = 200; max value = 3;  
                // set scale at 200/3 = 67  
                accelX.Value = acceleration.X;
                accelY.Value = acceleration.Y;
                accelZ.Value = acceleration.Z;

                Vector3 gravity = reading.Gravity;
                // height of control = 400; height of postive bar = 200; max value = 1;  
                // set scale at 200/1 = 200  
                gravityX.Value = gravity.X;
                gravityY.Value = gravity.Y;
                gravityZ.Value = gravity.Z;

                Vector3 rotation = reading.DeviceRotationRate;
                // height of control = 400; height of postive bar = 200; reasonable max value = 2pi = 6.25 (1.5 rotation per second)
                // set scale at 200/6.25 = 32
                gyroX.Value = rotation.X;
                gyroY.Value = rotation.Y;
                gyroZ.Value = rotation.Z;

                AttitudeReading attitude = reading.Attitude;
                attitudeX.Value = attitude.Pitch; // 0->pi 200/3.14 = 64
                attitudeY.Value = attitude.Roll;  // 0->pi/2 200/1.57 = 128
                attitudeZ.Value = attitude.Yaw;   // 0->2pi 200/6.28 = 32

                Vector3 worldSpacePoint = new Vector3(0.0f, 10.0f, 0.0f);
                Vector3 bodySpacePoint = Vector3.Transform(worldSpacePoint, attitude.RotationMatrix);
                point.Text = string.Format("Attitude: Transform of (0.0, 10.0, 0.0) = ({0:F1}, {1:F1}, {2:F1})",
                    bodySpacePoint.X, bodySpacePoint.Y, bodySpacePoint.Z);

            });
        }

        void sensor_Calibrate(object sender, CalibrationEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
                MessageBox.Show("The compass sensor needs to be calibrated. Wave the phone around in the air until the heading accuracy value is less than 20 degrees")
            );
        }

        private void start_Click(object sender, EventArgs e)
        {
            if (Motion.IsSupported)
                sensor.Start();
        }

        private void stop_Click(object sender, EventArgs e)
        {
            if (Motion.IsSupported)
                sensor.Stop();
        }
    }
}