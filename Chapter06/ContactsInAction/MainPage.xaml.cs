using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.UserData;

namespace ContactsInAction
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

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot pivot = (Pivot)sender;
            if (pivot.SelectedIndex == 1)
            {
                ApplicationBar = (ApplicationBar)Resources["appointmentsAppBar"];
            }
            else
            {
                ApplicationBar = (ApplicationBar)Resources["contactsAppBar"];  // initially = null;
            }
        }

        #region contacts pivot item code

        private void searchContacts_ActionIconTapped(object sender, EventArgs e)
        {
            FilterKind filterKind = FilterKind.DisplayName;
            if (phoneSearch.IsChecked.Value)
                filterKind = FilterKind.PhoneNumber;
            else if (emailSearch.IsChecked.Value)
                filterKind = FilterKind.EmailAddress;
            string filter = filterBox.Text;

            contacts.SearchAsync(filter, filterKind, null);
        }

        void contacts_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            try
            {
                var contact = e.Results.FirstOrDefault();
                var list = e.Results.ToList();
                contactsList.ItemsSource = e.Results.ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void addContact_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ContactPage.xaml", UriKind.Relative));
        }

        private void stackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            StackPanel layout = (StackPanel)sender;
            Contact contact = (Contact)layout.DataContext;
            Account firstAccount = contact.Accounts.First();
            if (firstAccount.Kind == StorageKind.Other && firstAccount.Name == "ContactsInAction")
            {
                String uri = string.Format("/ContactPage.xaml?givenName={0}&familyName={1}",
                    contact.CompleteName.FirstName, contact.CompleteName.LastName);
                NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            }
        }

        #endregion

        #region appointments pivot item code

        private void searchAppointments_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today.AddDays(1).AddSeconds(-1);

            if (sender == ApplicationBar.Buttons[1])
                end = end.AddDays(7);
            else if (sender == ApplicationBar.Buttons[2])
                end = end.AddDays(30);

            appointments.SearchAsync(start, end, null);
        }

        void appointments_SearchCompleted(object sender, AppointmentsSearchEventArgs e)
        {
            appointmentsList.ItemsSource = e.Results.ToList();
        }

        #endregion

        
    }
}