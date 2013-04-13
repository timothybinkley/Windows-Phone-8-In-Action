using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Advertising;
using Microsoft.Advertising.Mobile.UI;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AdvertisingSDK.Resources;
using System.Diagnostics;

namespace AdvertisingSDK
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            AdControl adcontrol = new AdControl()
                {
                    ApplicationId = "test_client",
                    AdUnitId = "TextAd",
                    Width = 480, 
                    Height = 80,
                    IsAutoRefreshEnabled = true,
                    IsAutoCollapseEnabled = true,
                    CountryOrRegion = CultureInfo.CurrentCulture.TwoLetterISOLanguageName 
                };
            adcontrol.ErrorOccurred += AdControl_OnErrorOccurred;
            adcontrol.AdRefreshed += AdControl_OnAdRefreshed;
            adcontrol.IsEngagedChanged += adcontrol_IsEngagedChanged;

            this.AdsStackPanel.Children.Add(adcontrol);

        }

        public void adcontrol_IsEngagedChanged(object sender, EventArgs e)
        {
            Console.WriteLine("User interacted with ad.");
            MessageBox.Show("User interacted with ad.");
        }

        public void AdControl_OnAdRefreshed(object sender, EventArgs e)
        {
            Console.WriteLine(((AdControl)sender).AdUnitId + " refreshed");
            MessageBox.Show(((AdControl) sender).AdUnitId + " refreshed");
        }

        public void AdControl_OnErrorOccurred(object sender, AdErrorEventArgs e)
        {
            Console.WriteLine("Error " + e.ErrorCode.ToString() + " on AdId: " + ((AdControl) sender).AdUnitId +
                                  ":: " + e.Error.Message);
            MessageBox.Show(e.Error.Message ,"Error "+ e.ErrorCode.ToString() + " on AdId: " + ((AdControl)sender).AdUnitId, MessageBoxButton.OK);
        }
    }
}