using System;
using System.Collections.Generic;
using System.Linq;

namespace Bazaarly.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; } = string.Empty; // ID of the user who placed the order
        public DateTime OrderDate { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public int AddressId { get; set; }
        public Addresses Addresses { get; set; }

        // New properties
        public string CustomerName { get; set; } // Name derived from UserId
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } // Set this based on your business logic
    }
}
