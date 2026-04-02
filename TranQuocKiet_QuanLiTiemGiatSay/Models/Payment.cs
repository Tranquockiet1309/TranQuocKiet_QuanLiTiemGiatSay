namespace TranQuocKiet_QuanLiTiemGiatSay.Models
{
    public class Payment
    {
        public long PaymentId { get; set; }
        public long OrderId { get; set; }
        public DateTime PaymentTime { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
        public string PaymentType { get; set; } = "FINAL";
        public long ReceivedBy { get; set; }
        public string? Note { get; set; }

        public Order? Order { get; set; }
        public User? Receiver { get; set; }
    }
}
