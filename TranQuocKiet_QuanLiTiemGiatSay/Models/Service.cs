namespace TranQuocKiet_QuanLiTiemGiatSay.Models
{
    public class Service
    {
        public long ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int? EstimatedHours { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
