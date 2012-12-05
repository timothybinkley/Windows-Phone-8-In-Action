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
    public partial class TransformGroupSample : PhoneApplicationPage
    {
        public TransformGroupSample()
        {
            InitializeComponent();
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new RotateTransform { Angle = 45 });
            transformGroup.Children.Add(new TranslateTransform { X = 20, Y = 30 });

            rectangleToTransform.RenderTransform = transformGroup;
        }
    }
}