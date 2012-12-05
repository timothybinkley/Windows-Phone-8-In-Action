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

namespace UserInterfaceControls
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["selection"] = pivot.SelectedIndex;
        }

        private void pivot_Loaded(object sender, RoutedEventArgs e)
        {
            int selection;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("selection", out selection))
            {
                pivot.SelectedIndex = selection;
            }
        }
    }
}