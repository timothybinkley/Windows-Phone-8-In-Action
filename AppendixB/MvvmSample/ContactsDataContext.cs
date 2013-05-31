using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmSample
{
    

    public class ContactsDataContext : DataContext
    {        
        public ContactsDataContext(string connectionString)
            : base(connectionString)
        { }
        
        public Table<ContactModel> Contacts;
    }
}
