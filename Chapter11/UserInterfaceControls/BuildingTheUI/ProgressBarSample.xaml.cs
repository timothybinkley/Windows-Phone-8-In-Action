using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Input;

namespace UserInterfaceControls.BuildingTheUI
{
    public partial class ProgressBarSample : PhoneApplicationPage
    {
        public ProgressBarSample()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            progressBar1.IsIndeterminate = !progressBar1.IsIndeterminate;
            progressBar1.Visibility = progressBar1.IsIndeterminate ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}