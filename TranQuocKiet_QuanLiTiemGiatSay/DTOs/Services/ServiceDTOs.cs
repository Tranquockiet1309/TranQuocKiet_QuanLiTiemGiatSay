namespace TranQuocKiet_QuanLiTiemGiatSay.DTOs.Services
{
    public class CreateServiceRequest
    {
        public string ServiceName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int? EstimatedHours { get; set; }
    }

    public class UpdateServiceRequest
    {
        public string ServiceName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int? EstimatedHours { get; set; }
        public bool IsActive { get; set; }
    }

    public class ServiceResponse
    {
        public long ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int? EstimatedHours { get; set; }
        public bool IsActive { get; set; }
    }
}
