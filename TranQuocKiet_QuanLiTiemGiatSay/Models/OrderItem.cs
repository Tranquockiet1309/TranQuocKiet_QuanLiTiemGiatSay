namespace TranQuocKiet_QuanLiTiemGiatSay.Models
{
    public class OrderItem
    {
        public long OrderItemId { get; set; }
        public long OrderId { get; set; }
        public long ServiceId { get; set; }
        public string? ItemDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineAmount { get; set; }
        public string ItemStatus { get; set; } = "PENDING";

        public Order? Order { get; set; }
        public Service? Service { get; set; }
    }
}
