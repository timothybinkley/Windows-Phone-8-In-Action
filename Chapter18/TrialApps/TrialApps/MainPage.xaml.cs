using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TrialApps.Resources;

namespace TrialApps
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (App.IsTrial)
            {             
                IsTrialTextBlock.Text = "Trial version";
                BuyButton.IsEnabled = true;
            }
            else
            {
                IsTrialTextBlock.Text = "Full version";
                BuyButton.IsEnabled = false;
            }
        }

        private void BuyButtonOnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This is where a MarketplaceTask would initiate the purchase of the full version",
                            "MarketplaceTask", MessageBoxButton.OK);
        }
    }
}