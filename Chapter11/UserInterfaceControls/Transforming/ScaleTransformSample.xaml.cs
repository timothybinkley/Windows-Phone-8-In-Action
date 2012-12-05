using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace UserInterfaceControls.Transforming
{
    public partial class ScaleTransformSample : PhoneApplicationPage
    {
        public ScaleTransformSample()
        {
            InitializeComponent();
            rectangleToScale.RenderTransform = new ScaleTransform
            {
                ScaleX = 0.5D,
                ScaleY = 0.75D,
                CenterX = 30.0D,
                CenterY = 60.0D
            };
        }
    }
}