using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Payments;
using TranQuocKiet_QuanLiTiemGiatSay.Services;

namespace TranQuocKiet_QuanLiTiemGiatSay.Controllers
{
    [Route("api/v1/payments")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("order/{orderId:long}")]
        public async Task<IActionResult> GetByOrder(long orderId)
        {
            var payments = await _paymentService.GetPaymentsByOrderIdAsync(orderId);
            return Ok(ApiResponse<IEnumerable<PaymentResponse>>.SuccessResponse(payments, "Lấy danh sách thanh toán thành công"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var userId = long.Parse(userIdClaim);
            var payment = await _paymentService.CreateAsync(request, userId);
            
            return Ok(ApiResponse<PaymentResponse>.SuccessResponse(payment, "Tạo thanh toán thành công"));
        }
    }
}
