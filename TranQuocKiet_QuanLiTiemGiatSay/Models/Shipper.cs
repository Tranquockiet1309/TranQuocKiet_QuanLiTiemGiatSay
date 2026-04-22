using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TranQuocKiet_QuanLiTiemGiatSay.Models
{
    public class Shipper
    {
        [Key]
        public long ShipperId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        // Link to User account for login
        public long? UserId { get; set; }
        public User? User { get; set; }

        // Navigation properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<DeliveryProof> DeliveryProofs { get; set; } = new List<DeliveryProof>();
    }
}
