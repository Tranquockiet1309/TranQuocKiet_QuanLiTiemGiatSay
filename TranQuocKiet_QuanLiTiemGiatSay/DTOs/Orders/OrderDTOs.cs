namespace TranQuocKiet_QuanLiTiemGiatSay.DTOs.Orders
{
    public class CreateOrderRequest
    {
        public long CustomerId { get; set; }
        public string ReceiveMethod { get; set; } = "AT_STORE";
        public DateTime? PromisedAt { get; set; }
        public decimal DiscountAmount { get; set; } = 0;
        public decimal DeliveryFee { get; set; } = 0;
        public string? OrderNote { get; set; }
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }

    public class CreateOrderItemRequest
    {
        public long ServiceId { get; set; }
        public decimal Quantity { get; set; }
        public string? ItemDescription { get; set; }
    }

    public class OrderResponse
    {
        public long OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime ReceivedAt { get; set; }
        public DateTime? PromisedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public List<OrderItemResponse> Items { get; set; } = new();
    }

    public class OrderItemResponse
    {
        public long OrderItemId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineAmount { get; set; }
        public string ItemStatus { get; set; } = string.Empty;
    }
}
