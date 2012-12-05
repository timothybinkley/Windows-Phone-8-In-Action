using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace Panorama
{
    public partial class PanoramaPage1 : PhoneApplicationPage
    {
        public PanoramaPage1()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            int selectedIndex;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("selection", out selectedIndex))
            {
                panorama.DefaultItem = panorama.Items[selectedIndex];
            }
        }

        private void panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["selection"] = panorama.SelectedIndex;
        }
    }
}