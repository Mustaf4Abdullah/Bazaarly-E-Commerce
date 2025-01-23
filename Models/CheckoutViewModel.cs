namespace Bazaarly.Models
{
    public class CheckoutViewModel
    {
        public IEnumerable<CartItem> CartItems { get; set; }
        public List<Addresses> Addresses { get; set; }
        public int SelectedAddressId { get; set; } // To hold the selected address ID
    }
}
