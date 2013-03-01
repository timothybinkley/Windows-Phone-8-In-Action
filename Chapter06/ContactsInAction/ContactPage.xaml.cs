using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Windows.Phone.PersonalInformation;

namespace ContactsInAction
{
    public partial class ContactPage : PhoneApplicationPage
    {
        ContactStore store;
        StoredContact contact;

        public ContactPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            store = await ContactStore.CreateOrOpenAsync(   // initally created with default parameters
               ContactStoreSystemAccessMode.ReadWrite,
               ContactStoreApplicationAccessMode.ReadOnly);

            if (NavigationContext.QueryString.ContainsKey("givenName") &&
                NavigationContext.QueryString.ContainsKey("familyName"))
            {
                string givenName = NavigationContext.QueryString["givenName"];
                string familyName = NavigationContext.QueryString["familyName"];
                FindAndLoadContact(givenName, familyName);
            }
        }

        private async void Save_Click(object sender, EventArgs e)
        {
            if (contact == null)
            {
                contact = new StoredContact(store);
            }

            IDictionary<string, object> properties = await contact.GetPropertiesAsync();
            contact.GivenName = firstNameInput.Text;
            contact.FamilyName = lastNameInput.Text;
            properties[KnownContactProperties.Email] = emailInput.Text;
            properties[KnownContactProperties.Telephone] = phoneInput.Text;

            await contact.SaveAsync();
            NavigationService.GoBack();
        }

        private async void Delete_Click(object sender, EventArgs e)
        {
            if (contact != null)
            {
                await store.DeleteContactAsync(contact.Id);
            }
            NavigationService.GoBack();
        }

        private async void FindAndLoadContact(string givenName, string familyName)
        {
            ContactQueryOptions options = new ContactQueryOptions();
            options.OrderBy = ContactQueryResultOrdering.GivenNameFamilyName;
            options.DesiredFields.Clear();
            options.DesiredFields.Add(KnownContactProperties.GivenName);
            options.DesiredFields.Add(KnownContactProperties.FamilyName);
            options.DesiredFields.Add(KnownContactProperties.Email);
            options.DesiredFields.Add(KnownContactProperties.Telephone);

            ContactQueryResult query = store.CreateContactQuery(options);
            IReadOnlyList<StoredContact> contacts = await query.GetContactsAsync();
            contact = contacts.First(item =>
                item.GivenName == givenName && item.FamilyName == familyName);

            IDictionary<string, object> props = await contact.GetPropertiesAsync();
            firstNameInput.Text = contact.GivenName;
            lastNameInput.Text = contact.FamilyName;
            if (props.ContainsKey(KnownContactProperties.Email))
                emailInput.Text = (string)props[KnownContactProperties.Email];
            if (props.ContainsKey(KnownContactProperties.Telephone))
                phoneInput.Text = (string)props[KnownContactProperties.Telephone];
        }
    }
}