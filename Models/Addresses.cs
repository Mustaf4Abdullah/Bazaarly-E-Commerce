using System.ComponentModel.DataAnnotations;

namespace Bazaarly.Models
{
    public class Addresses
    {
        public int AddressId { get; set; }
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string State { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string ZipCode { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Country { get; set; } = string.Empty;

        public string FullAddress => $"{Street}, {City}, {State}, {ZipCode}, {Country}";
    }
}
