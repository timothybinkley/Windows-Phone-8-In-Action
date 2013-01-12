using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.UserData;

namespace UserData
{
    public partial class MainPage : PhoneApplicationPage
    {
        Contacts contacts = new Contacts();
        Appointments appointments = new Appointments();

        public MainPage()
        {
            InitializeComponent();
            contacts.SearchCompleted += contacts_SearchCompleted;
            appointments.SearchCompleted += appointments_SearchCompleted;
        }

        #region contacts pivot item code

        private void searchContacts_Click(object sender, RoutedEventArgs e)
        {
            var filterKind = FilterKind.DisplayName;
            if (phoneSearch.IsChecked.Value)
                filterKind = FilterKind.PhoneNumber;
            else if (emailSearch.IsChecked.Value)
                filterKind = FilterKind.EmailAddress;

            string filter = filterBox.Text;

            contacts.SearchAsync(filter, filterKind, null);
        }

        void contacts_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            searchContactsResult.Text = string.Format("{0} contacts found", e.Results.Count());
            if (e.Results.Count() > 1)
                searchContactsResult.Text += string.Format(", displaying the first match");
            contactControl.Content = e.Results.FirstOrDefault();
        }

        #endregion

        #region appointments pivot item code

        private void searchAppointments_Click(object sender, RoutedEventArgs e)
        {
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today.AddDays(1).AddSeconds(-1);

            if (weekSearch.IsChecked.Value)
                end = end.AddDays(7);
            else if (monthSearch.IsChecked.Value)
                end = end.AddDays(30);

            appointments.SearchAsync(start, end, null);
        }

        void appointments_SearchCompleted(object sender, AppointmentsSearchEventArgs e)
        {
            apptsResult.Text = string.Format("{0} appointments found", e.Results.Count());
            if (e.Results.Count() > 1)
                apptsResult.Text += string.Format(", displaying the first match");
            appointmentControl.Content = e.Results.FirstOrDefault();
        }

        #endregion
    }
}