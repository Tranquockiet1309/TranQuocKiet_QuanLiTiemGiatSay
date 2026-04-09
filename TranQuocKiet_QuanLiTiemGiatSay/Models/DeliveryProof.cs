using System.ComponentModel.DataAnnotations;

namespace TranQuocKiet_QuanLiTiemGiatSay.Models
{
    public class DeliveryProof
    {
        [Key]
        public long Id { get; set; }

        public long OrderId { get; set; }
        public long ShipperId { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Order? Order { get; set; }
        public Shipper? Shipper { get; set; }
    }
}
