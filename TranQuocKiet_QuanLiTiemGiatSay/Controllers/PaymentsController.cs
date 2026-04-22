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
        private readonly IVnPayService _vnPayService;

        public PaymentsController(IPaymentService paymentService, IVnPayService vnPayService)
        {
            _paymentService = paymentService;
            _vnPayService = vnPayService;
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

        [HttpPost("vnpay/create-url")]
        [AllowAnonymous]
        public IActionResult CreateVnPayUrl([FromBody] VnPayRequest request)
        {
            var url = _vnPayService.CreatePaymentUrl(HttpContext, request);
            return Ok(ApiResponse<string>.SuccessResponse(url, "Tạo URL thanh toán VNPay thành công"));
        }

        [HttpGet("vnpay/callback")]
        [AllowAnonymous]
        public async Task<IActionResult> VnPayCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (!response.Success)
            {
                return BadRequest(ApiResponse.ErrorResponse("Thanh toán VNPay thất bại hoặc chữ ký không hợp lệ. Mã lỗi: " + response.VnPayResponseCode));
            }

            if (!long.TryParse(response.OrderId, out long orderId))
            {
                return BadRequest(ApiResponse.ErrorResponse("Mã đơn hàng không hợp lệ từ VNPay."));
            }

            // Dùng userId = 1 (system/owner) vì không có token từ VNPay redirect
            // Lấy userId từ token nếu có
            long userId = 1;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdClaim))
            {
                userId = long.Parse(userIdClaim);
            }

            var vnpAmount = Request.Query["vnp_Amount"].ToString();
            if (!decimal.TryParse(vnpAmount, out decimal rawAmount))
            {
                return BadRequest(ApiResponse.ErrorResponse("Số tiền thanh toán không hợp lệ."));
            }

            var paymentRequest = new CreatePaymentRequest
            {
                OrderId = orderId,
                Amount = rawAmount / 100, // VNPay nhân 100 khi gửi sang
                Method = "EWallet",
                PaymentType = "FINAL",
                Note = $"VNPay - Mã GD: {response.TransactionId}"
            };

            try
            {
                await _paymentService.CreateAsync(paymentRequest, userId);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.ErrorResponse($"Lỗi khi ghi nhận thanh toán: {ex.Message}"));
            }

            return Ok(ApiResponse<VnPayResponse>.SuccessResponse(response, "Thanh toán VNPay thành công"));
        }
    }
}
