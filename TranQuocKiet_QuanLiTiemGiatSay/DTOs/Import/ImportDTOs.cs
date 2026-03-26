using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Orders;

namespace TranQuocKiet_QuanLiTiemGiatSay.DTOs.Import
{
    public class BatchImportOrderRequest
    {
        public List<CreateOrderRequest> Orders { get; set; } = new();
    }

    public class ImportResult
    {
        public int Total { get; set; }
        public int Success { get; set; }
        public int Failed { get; set; }
        public List<ImportError> Errors { get; set; } = new();
    }

    public class ImportError
    {
        public int Row { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
