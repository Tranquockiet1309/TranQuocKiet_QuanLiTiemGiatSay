using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Inventory;
using TranQuocKiet_QuanLiTiemGiatSay.Services;

namespace TranQuocKiet_QuanLiTiemGiatSay.Controllers
{
    [Route("api/v1/inventory")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var txns = await _inventoryService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<InventoryTxnResponse>>.SuccessResponse(txns, "Lấy danh sách giao dịch kho thành công"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInventoryTxnRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var userId = long.Parse(userIdClaim);
            var txn = await _inventoryService.CreateAsync(request, userId);
            
            return Ok(ApiResponse<InventoryTxnResponse>.SuccessResponse(txn, "Tạo giao dịch kho thành công"));
        }
    }
}
