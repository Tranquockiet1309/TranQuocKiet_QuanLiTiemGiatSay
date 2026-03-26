using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TranQuocKiet_QuanLiTiemGiatSay.Constants;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Import;
using TranQuocKiet_QuanLiTiemGiatSay.Services;

namespace TranQuocKiet_QuanLiTiemGiatSay.Controllers
{
    [Route("api/v1/import")]
    [ApiController]
    [Authorize(Roles = Roles.OWNER)]
    public class ImportController : ControllerBase
    {
        private readonly IImportService _importService;

        public ImportController(IImportService importService)
        {
            _importService = importService;
        }

        [HttpPost("orders")]
        public async Task<IActionResult> ImportOrders([FromBody] BatchImportOrderRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var userId = long.Parse(userIdClaim);
            var result = await _importService.ImportOrdersAsync(request, userId);
            
            return Ok(ApiResponse<ImportResult>.SuccessResponse(result, "Xử lý import hoàn tất"));
        }
    }
}
