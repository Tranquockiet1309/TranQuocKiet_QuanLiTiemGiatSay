using System.ComponentModel.DataAnnotations;

namespace TranQuocKiet_QuanLiTiemGiatSay.Models
{
    public class User
    {
        [Key]
        public long UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "STAFF";
        public bool IsActive { get; set; } = true;
        // 🔥 Link sang Customer
        public Customer? Customer { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
