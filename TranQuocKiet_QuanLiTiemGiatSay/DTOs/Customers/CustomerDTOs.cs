namespace TranQuocKiet_QuanLiTiemGiatSay.DTOs.Customers
{
    public class CreateCustomerRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? CustomerNote { get; set; }
    }

    public class UpdateCustomerRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? CustomerNote { get; set; }
    }

    public class CustomerResponse
    {
        public long CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? CustomerNote { get; set; }
        public decimal TotalSpent { get; set; }
        public int PointBalance { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
