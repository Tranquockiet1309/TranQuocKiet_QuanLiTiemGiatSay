namespace TranQuocKiet_QuanLiTiemGiatSay.Models
{
    public class Customer
    {
        public long CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? CustomerNote { get; set; }
        public decimal TotalSpent { get; set; } = 0;
        public int PointBalance { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
