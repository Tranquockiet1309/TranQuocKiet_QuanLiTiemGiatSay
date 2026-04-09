using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using TranQuocKiet_QuanLiTiemGiatSay.Data;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Delivery;
using TranQuocKiet_QuanLiTiemGiatSay.Hubs;
using TranQuocKiet_QuanLiTiemGiatSay.Models;

namespace TranQuocKiet_QuanLiTiemGiatSay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IHubContext<OrderHub> _hubContext;

        public virtual string UploadPath => Path.Combine(_environment.WebRootPath, "uploads", "delivery_proofs");

        public DeliveryController(
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            IHubContext<OrderHub> hubContext)
        {
            _context = context;
            _environment = environment;
            _hubContext = hubContext;
        }

        [HttpPost("upload-proof")]
        public async Task<IActionResult> UploadProof([FromForm] DeliveryProofRequest request)
        {
            var order = await _context.Orders
                .Include(o => o.DeliveryProof)
                .FirstOrDefaultAsync(o => o.OrderId == request.OrderId);

            if (order == null)
            {
                return NotFound(ApiResponse<string>.ErrorResponse("Không tìm thấy đơn hàng"));
            }

            if (order.DeliveryProof != null)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Đơn hàng đã có ảnh xác nhận giao hàng"));
            }

            var shipper = await _context.Shippers.FindAsync(request.ShipperId);
            if (shipper == null)
            {
                return NotFound(ApiResponse<string>.ErrorResponse("Không tìm thấy shipper"));
            }

            // Save image
            if (!Directory.Exists(UploadPath))
            {
                Directory.CreateDirectory(UploadPath);
            }

            var fileName = $"{order.OrderCode}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(request.Image.FileName)}";
            var filePath = Path.Combine(UploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Image.CopyToAsync(stream);
            }

            var imageUrl = $"/uploads/delivery_proofs/{fileName}";

            // Create DeliveryProof
            var proof = new DeliveryProof
            {
                OrderId = request.OrderId,
                ShipperId = request.ShipperId,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.DeliveryProofs.Add(proof);

            // Update Order
            order.Status = OrderStatus.Delivered;
            order.CompletedAt = DateTime.Now;
            order.ShipperId = request.ShipperId;

            await _context.SaveChangesAsync();

            // Notify Admin
            await _hubContext.Clients.Group("Admins").SendAsync("OrderDelivered", new
            {
                order.OrderId,
                order.OrderCode,
                order.Status,
                ImageUrl = imageUrl,
                ShipperName = shipper.Name
            });

            return Ok(ApiResponse<DeliveryProofResponse>.SuccessResponse(new DeliveryProofResponse
            {
                Id = proof.Id,
                OrderId = proof.OrderId,
                ImageUrl = proof.ImageUrl,
                CreatedAt = proof.CreatedAt
            }, "Tải lên ảnh xác nhận thành công"));
        }

        [HttpGet("assigned-orders/{shipperId}")]
        public async Task<IActionResult> GetAssignedOrders(long shipperId)
        {
            var orders = await _context.Orders
                .Where(o => o.ShipperId == shipperId && o.Status != OrderStatus.Delivered && o.Status != OrderStatus.Cancelled)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderCode,
                    o.Status,
                    o.TotalAmount,
                    CustomerName = o.Customer != null ? o.Customer.FullName : "N/A",
                    o.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.SuccessResponse(orders, "Lấy danh sách đơn hàng thành công"));
        }

        [HttpGet("shippers")]
        public async Task<IActionResult> GetShippers()
        {
            var shippers = await _context.Shippers
                .Select(s => new ShipperResponse
                {
                    ShipperId = s.ShipperId,
                    Name = s.Name,
                    Phone = s.Phone
                })
                .ToListAsync();

            return Ok(ApiResponse<List<ShipperResponse>>.SuccessResponse(shippers, "Lấy danh sách shipper thành công"));
        }

        [HttpPost("assign-shipper")]
        public async Task<IActionResult> AssignShipper([FromBody] AssignShipperRequest request)
        {
            var order = await _context.Orders.FindAsync(request.OrderId);
            if (order == null)
            {
                return NotFound(ApiResponse<string>.ErrorResponse("Không tìm thấy đơn hàng"));
            }

            if (order.Status != OrderStatus.Completed)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Chỉ có thể gán shipper cho đơn hàng đã hoàn thành (Xử lý xong)"));
            }

            var shipper = await _context.Shippers.FindAsync(request.ShipperId);
            if (shipper == null)
            {
                return NotFound(ApiResponse<string>.ErrorResponse("Không tìm thấy shipper"));
            }

            order.ShipperId = request.ShipperId;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<string>.SuccessResponse("Gán shipper thành công"));
        }
    }
}
