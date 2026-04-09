using Microsoft.AspNetCore.Http;

namespace TranQuocKiet_QuanLiTiemGiatSay.DTOs.Delivery
{
    public class DeliveryProofRequest
    {
        public long OrderId { get; set; }
        public long ShipperId { get; set; }
        public IFormFile Image { get; set; } = null!;
    }

    public class DeliveryProofResponse
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ShipperResponse
    {
        public long ShipperId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class AssignShipperRequest
    {
        public long OrderId { get; set; }
        public long ShipperId { get; set; }
    }
}
