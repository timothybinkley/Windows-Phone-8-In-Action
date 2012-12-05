using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace UserInterfaceControls.Toolkit
{
    public partial class ContextmenuSample : PhoneApplicationPage
    {
        public ContextmenuSample()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            MessageBox.Show(menuItem.Name, "Menu Item Clicked", MessageBoxButton.OK);
        }

        private void menuItem2Enabled_Clicked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            menuItem2.IsEnabled = checkBox.IsChecked.Value;
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("menu opened");
        }
    }
}