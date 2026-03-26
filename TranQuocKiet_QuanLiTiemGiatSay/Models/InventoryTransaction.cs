namespace TranQuocKiet_QuanLiTiemGiatSay.Models
{
    public class InventoryTransaction
    {
        public long InventoryTxnId { get; set; }
        public DateTime TxnDate { get; set; } = DateTime.Now;
        public string ItemName { get; set; } = string.Empty;
        public string TxnType { get; set; } = "IN";
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal? UnitCost { get; set; }
        public string? ReferenceNote { get; set; }
        public long CreatedBy { get; set; }

        public User? Creator { get; set; }
    }
}
