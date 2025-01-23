
namespace Bazaarly.Models
{
    public class Favorites
    {
        public int FavoritesId { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; } 

        public Product Product { get; set; } 
    }
}
