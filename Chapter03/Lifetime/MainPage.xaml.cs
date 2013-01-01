using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Lifetime.Resources;
using System.Windows.Input;
using System.Windows.Threading;

namespace Lifetime
{
    public partial class MainPage : PhoneApplicationPage
    {
        DispatcherTimer timer;

        DateTime pageConstructedTime;
        DateTime navigatedToTime;
        DateTime navigatedFromTime;
        DateTime obscuredTime;
        DateTime unobscuredTime;
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Start();

            pageConstructedTime = DateTime.Now;

            App.RootFrame.Obscured += RootFrame_Obscured;
            App.RootFrame.Unobscured += RootFrame_Unobscured;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            UpdateUserInterface();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        private void UpdateUserInterface()
        {
            DateTime now = DateTime.Now;

            // page level times
            pageConstructed.DataContext = (now - pageConstructedTime).TotalSeconds;
            navigatedTo.DataContext = (now - navigatedToTime).TotalSeconds;

            if (navigatedFromTime != DateTime.MinValue)
                navigatedFrom.DataContext = (now - navigatedFromTime).TotalSeconds;

            if (obscuredTime != DateTime.MinValue)
                obscured.DataContext = (now - obscuredTime).TotalSeconds;

            if (unobscuredTime != DateTime.MinValue)
                unobscured.DataContext = (now - unobscuredTime).TotalSeconds;

            // app level times
            var app = (App)Application.Current;

            appConstructed.DataContext = (now - app.AppConstructedTime).TotalSeconds;
            launched.DataContext = (now - app.LaunchedTime).TotalSeconds;

            if (app.DeactivatedTime != DateTime.MinValue)
                deactivated.DataContext = (now - app.DeactivatedTime).TotalSeconds;

            if (app.ActivatedTime != DateTime.MinValue)
                activated.DataContext = (now - app.ActivatedTime).TotalSeconds;

            instancePreserved.Text = app.IsApplicationInstancePreserved.ToString();

            lockscreen.IsEnabled = PhoneApplicationService.Current.ApplicationIdleDetectionMode == IdleDetectionMode.Enabled;
            //navCanceled.Text = app.NavigationCanceled.ToString();
            //stackCleared.Text = app.BackStackCleared.ToString();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("MainPage.OnNavigatedTo (" + e.NavigationMode + "): " + e.Uri);

            navigatedToTime = DateTime.Now;

            var app = (App)Application.Current;
            bool appInstancePreserved = app.IsApplicationInstancePreserved ?? true;

            if (!appInstancePreserved && State.ContainsKey("NavigatedFromTime"))
            {
                navigatedFromTime = (DateTime)State["NavigatedFromTime"];
            }

            //UpdateUserInterface();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (!e.IsNavigationInitiator)
            {
                navigatedFromTime = DateTime.Now;
                State["NavigatedFromTime"] = navigatedFromTime;
            }
            base.OnNavigatedFrom(e);
        }

        private void lockscreen_Checked(object sender, RoutedEventArgs e)
        {
            PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
            //lockscreen.IsEnabled = false;
        }

        void RootFrame_Obscured(object sender, ObscuredEventArgs e)
        {
            obscuredTime = DateTime.Now;
            if (e.IsLocked)
                timer.Stop();
            //UpdateUserInterface();
        }
        void RootFrame_Unobscured(object sender, EventArgs e)
        {
            unobscuredTime = DateTime.Now;
            timer.Start();
            //UpdateUserInterface();
        }

        private void pin_Click(object sender, EventArgs e)
        {
            int count = ShellTile.ActiveTiles.Count();
            var tileData = new FlipTileData
             {
                 Title = "Lifetime",
                 Count = count,
             };
            Uri launchUrl = new Uri(String.Format("/MainPage.xaml?context={0}", count), UriKind.Relative);
            ShellTile.Create(launchUrl, tileData, false);
        }

    }
}