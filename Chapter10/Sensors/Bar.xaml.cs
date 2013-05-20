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

namespace Sensors
{
    public partial class Bar : UserControl
    {
        public Bar()
        {
            InitializeComponent();
        }

        public Brush BarFill
        {
            get { return positiveBar.Fill; }
            set
            {
                positiveBar.Fill = value;
                negativeBar.Fill = value;
            }
        }

        private double scale;

        public double Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                Update();
            }
        }

        private double barValue;

        public double Value
        {
            get { return barValue; }
            set
            {
                barValue = value;
                Update();
            }
        }

        private void Update()
        {
            int height = (int)(barValue * scale);
            positiveBar.Height = height > 0 ? height : 0;
            negativeBar.Height = height < 0 ? height * -1 : 0;
            label.Text = barValue.ToString("0.0");
        }

    }
}
