using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace Bazaarly.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } 
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;

        // Relationships
        public List<Comments> Comments { get; set; } = new List<Comments>();
        public virtual ICollection<Image> Images { get; set; } = new List<Image>();
    }
}
