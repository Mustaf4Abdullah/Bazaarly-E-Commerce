namespace Bazaarly.Models
{
    public class CartItem
    {
        public int Id { get; set; } // This is the primary key for the CartItem table
        public string UserId { get; set; } // Foreign key for the User
        public int ProductId { get; set; } // Foreign key for the Product
        public Product Product { get; set; } // Navigation property to the Product
        public int Quantity { get; set; } // Quantity of the product in the cart
    }
}
