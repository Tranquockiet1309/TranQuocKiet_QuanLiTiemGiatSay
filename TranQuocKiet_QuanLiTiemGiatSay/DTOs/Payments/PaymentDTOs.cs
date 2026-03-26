namespace TranQuocKiet_QuanLiTiemGiatSay.DTOs.Payments
{
    public class CreatePaymentRequest
    {
        public long OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = "CASH";
        public string PaymentType { get; set; } = "FINAL"; // PREPAYMENT or FINAL
        public string? Note { get; set; }
    }

    public class PaymentResponse
    {
        public long PaymentId { get; set; }
        public long OrderId { get; set; }
        public DateTime PaymentTime { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public long ReceivedBy { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string? Note { get; set; }
    }
}
