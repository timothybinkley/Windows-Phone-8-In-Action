using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmSample
{
    public class ContactViewModel : INotifyPropertyChanged
    {
        public ContactViewModel()
        {
            Contacts = new ObservableCollection<ContactModel>
            {                 
                new ContactModel() { Id = 1, FirstName = "Tifa", LastName = "Lockhart", PhoneNumber = "91988872" },
                new ContactModel() { Id = 2, FirstName = "Aerith", LastName = "Gainsborough" , PhoneNumber = "87366790"},
                new ContactModel() { Id = 3, FirstName = "Cloud", LastName = "Strife" , PhoneNumber = "46738633"},
                new ContactModel() { Id = 4, FirstName = "Aki", LastName = "Ross" , PhoneNumber = "776463839"},
            };

            Contact = new ContactModel();

            SaveContactCommand = new ActionCommand(() =>
            {
                Contacts.Add(new ContactModel() { Id = Contacts.Count() + 1, FirstName = Contact.FirstName, LastName = Contact.LastName, PhoneNumber = Contact.PhoneNumber });
                Contact = new ContactModel();
            });
        }

          

        private ObservableCollection<ContactModel> _contacts;
        public ObservableCollection<ContactModel> Contacts
        {
            get { return _contacts; }
            set
            {
                _contacts = value;
                NotifyPropertyChanged("Contacts");
            }
        }

        private ContactModel _contact;
        public ContactModel Contact
        {
            get { return _contact; }
            set
            {
                _contact = value;
                NotifyPropertyChanged("Contact");
            }
        }

        
        public ICommand SaveContactCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the app that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

}
