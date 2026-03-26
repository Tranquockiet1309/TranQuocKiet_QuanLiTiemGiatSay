namespace TranQuocKiet_QuanLiTiemGiatSay.DTOs.Inventory
{
    public class CreateInventoryTxnRequest
    {
        public string ItemName { get; set; } = string.Empty;
        public string TxnType { get; set; } = "IN"; // IN or OUT
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal? UnitCost { get; set; }
        public string? ReferenceNote { get; set; }
    }

    public class InventoryTxnResponse
    {
        public long InventoryTxnId { get; set; }
        public DateTime TxnDate { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string TxnType { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal? UnitCost { get; set; }
        public string? ReferenceNote { get; set; }
        public long CreatedBy { get; set; }
        public string CreatorName { get; set; } = string.Empty;
    }
}
