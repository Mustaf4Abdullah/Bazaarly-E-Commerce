using System.Collections.Generic;
using System.Net;

namespace Bazaarly.Models
{
    public class UserAccountViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }

        // List of orders associated with the user
        public List<Order> OrderHistory { get; set; } 

        // List of saved addresses
        public List<Addresses> SavedAddresses { get; set; } 
    }
}
