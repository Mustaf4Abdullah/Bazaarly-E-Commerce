namespace Bazaarly.Models
{
    public class Image
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
