using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Orders;
using TranQuocKiet_QuanLiTiemGiatSay.Services;

namespace TranQuocKiet_QuanLiTiemGiatSay.Controllers
{
    [Route("api/v1/orders")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<OrderResponse>>.SuccessResponse(orders, "Lấy danh sách đơn hàng thành công"));
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound(ApiResponse.ErrorResponse("Không tìm thấy đơn hàng"));
            }
            return Ok(ApiResponse<OrderResponse>.SuccessResponse(order, "Lấy thông tin đơn hàng thành công"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var userId = long.Parse(userIdClaim);
            var order = await _orderService.CreateAsync(request, userId);
            
            return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, 
                ApiResponse<OrderResponse>.SuccessResponse(order, "Tạo đơn hàng thành công"));
        }

        [HttpPut("{id:long}/status")]
        public async Task<IActionResult> UpdateStatus(long id, [FromQuery] string status)
        {
            var order = await _orderService.UpdateStatusAsync(id, status);
            if (order == null)
            {
                return NotFound(ApiResponse.ErrorResponse("Không tìm thấy đơn hàng"));
            }
            return Ok(ApiResponse<OrderResponse>.SuccessResponse(order, "Cập nhật trạng thái đơn hàng thành công"));
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var success = await _orderService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse.ErrorResponse("Không tìm thấy đơn hàng"));
            }
            return Ok(ApiResponse.SuccessResponse("Xóa đơn hàng thành công"));
        }
    }
}
