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

namespace UserInterfaceControls.ReceivingInput
{
    public partial class InputScopeSample : PhoneApplicationPage
    {
        public InputScopeSample()
        {
            InitializeComponent();
            InputScope inputScope = new InputScope();
            inputScope.Names.Add(new InputScopeName { NameValue = InputScopeNameValue.TelephoneNumber });
            textBox1.InputScope = inputScope;
        }
    }
}