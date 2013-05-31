using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmSample
{
    public class ContactModel :  INotifyPropertyChanged    
    {        
        private int _id;
        
        public int Id
        {
            get { return _id; }
            set
            {                
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }
        
        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set
            {                
                _firstName = value;
                NotifyPropertyChanged("FirstName");
            }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                NotifyPropertyChanged("LastName");
            }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                _phoneNumber = value;
                NotifyPropertyChanged("PhoneNumber");
            }
        }
        

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }      
        
    }
}
