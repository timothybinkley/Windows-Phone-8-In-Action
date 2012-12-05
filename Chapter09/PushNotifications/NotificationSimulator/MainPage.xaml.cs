using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Net.NetworkInformation;
using System.IO.IsolatedStorage;
using System.Threading;


namespace NotificationSimulator
{
    public partial class MainPage : PhoneApplicationPage
    {
        NotificationService service = new NotificationService();

        public MainPage()
        {
            InitializeComponent();

            DeviceNetworkInformation.NetworkAvailabilityChanged +=
                (sender, e) => LoadInformation(e.NotificationType.ToString());

            string savedUri;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("ChannelUri", out savedUri))
            {
                channelUri.Text = savedUri;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadInformation("OnNagivatedTo");
        }

        private void LoadInformation(string trigger)
        {
            //System.Diagnostics.Debug.WriteLine(trigger);
            statusMessage.Text = "loading...";
            ThreadPool.QueueUserWorkItem((state) =>
            {
                string information = string.Format("Information triggered by: {0}\n\n{1}\n{2}",
                    trigger,
                    LoadPhoneNetworkInformation(),
                    LoadDeviceInformation());

                Dispatcher.BeginInvoke(() => statusMessage.Text = information);
            });
        }

        private string LoadPhoneNetworkInformation()
        {
            return string.Format("Phone network type: {0}\nPhone network available: {1}\n",
                NetworkInterface.NetworkInterfaceType,
                NetworkInterface.GetIsNetworkAvailable());
        }

        private string LoadDeviceInformation()
        {
            return string.Format("Cellular operator: {0}\nCellular data enabled: {1}\n" +
                "Roaming enabled: {2}\nDevice network available: {3}\nWi-Fi enabled: {4}\n",
                DeviceNetworkInformation.CellularMobileOperator,
                DeviceNetworkInformation.IsCellularDataEnabled,
                DeviceNetworkInformation.IsCellularDataRoamingEnabled,
                DeviceNetworkInformation.IsNetworkAvailable,
                DeviceNetworkInformation.IsWiFiEnabled);
        }

        private void sendToast_Click(object sender, RoutedEventArgs e)
        {
            Uri channel;
            if (Uri.TryCreate(channelUri.Text, UriKind.Absolute, out channel))
            {
                service.SendToast(channel, toastTitle.Text, toastContent.Text,
                    string.Format("/MainPage.xaml?value1={0}&amp;value2={1}", toastValue1.Text, toastvalue2.Text));
            }
        }

        private void sendTile_Click(object sender, RoutedEventArgs e)
        {
            Uri channel;
            if (Uri.TryCreate(channelUri.Text, UriKind.Absolute, out channel))
            {
                string imagePath = tileDefaultImage.IsChecked.Value ? "Assets/Tiles/FlipCycleTileMedium.png" : tileBlueImage.IsChecked.Value ? "Assets/Tiles/Blue.jpg" : "Assets/Tiles/Green.jpg";
                string backImagePath = tileBackNoImage.IsChecked.Value ? "" : tileBackBlueImage.IsChecked.Value ? "Assets/Tiles/Blue.jpg" : "Assets/Tiles/Green.jpg";
                int badgeCount;
                Int32.TryParse(tileBadgeCount.Text, out badgeCount);
                service.SendTile(channel, imagePath, badgeCount, tileTitle.Text, backImagePath, tileBackTitle.Text, tileBackContent.Text);
            }
        }

        private void stackPanel_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["ChannelUri"] = channelUri.Text;
        }
    }
}