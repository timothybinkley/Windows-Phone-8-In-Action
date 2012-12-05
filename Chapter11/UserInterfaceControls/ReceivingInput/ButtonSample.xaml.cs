using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace UserInterfaceControls.ReceivingInput
{
    public partial class ButtonSample : PhoneApplicationPage
    {
        public ButtonSample()
        {
            InitializeComponent();
        }
        private void releaseButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("release click");
        }

        private void pressButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("press click");
        }

        private void hoverButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("hover click");
        }
    }
}