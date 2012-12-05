using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;

namespace UserInterfaceControls.BuildingTheUI
{
    public partial class ImageSample : PhoneApplicationPage
    {
        public ImageSample()
        {
            InitializeComponent();

            image3.Source = new BitmapImage(
                new Uri("http://www.wp7inaction.com/cover.png",
                UriKind.Absolute));
        }
    }
}