namespace TranQuocKiet_QuanLiTiemGiatSay.Models
{
    public class Order
    {
        public long OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public long CustomerId { get; set; }
        public long ReceivedBy { get; set; }
        public string ReceiveMethod { get; set; } = "AT_STORE";
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ReceivedAt { get; set; } = DateTime.Now;
        public DateTime? PromisedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public decimal SubtotalAmount { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;
        public decimal DeliveryFee { get; set; } = 0;
        public decimal TotalAmount { get; set; } = 0;
        public decimal PaidAmount { get; set; } = 0;
        public string PaymentStatus { get; set; } = "UNPAID";
        public string? OrderNote { get; set; }
        public long? ShipperId { get; set; }

        public Customer? Customer { get; set; }
        public User? Receiver { get; set; }
        public Shipper? Shipper { get; set; }
        public DeliveryProof? DeliveryProof { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
