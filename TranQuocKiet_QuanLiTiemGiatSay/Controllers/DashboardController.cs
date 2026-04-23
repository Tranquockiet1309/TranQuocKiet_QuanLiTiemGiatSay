using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TranQuocKiet_QuanLiTiemGiatSay.Constants;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Dashboard;
using TranQuocKiet_QuanLiTiemGiatSay.Services;

namespace TranQuocKiet_QuanLiTiemGiatSay.Controllers
{
    [Route("api/v1/dashboard")]
    [ApiController]
    [Authorize(Roles = Roles.OWNER)]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var start = startDate ?? DateTime.Now.AddDays(-30);
            var end = endDate ?? DateTime.Now;

            var stats = await _dashboardService.GetStatsAsync(start, end);
            return Ok(ApiResponse<DashboardStatsResponse>.SuccessResponse(stats, "Lấy số liệu thống kê thành công"));
        }

        [HttpGet("monthly-summary")]
        public async Task<IActionResult> GetMonthlySummary([FromQuery] int months = 5)
        {
            if (months < 1 || months > 24) months = 5;
            var data = await _dashboardService.GetMonthlySummaryAsync(months);
            return Ok(ApiResponse<List<MonthlyMetric>>.SuccessResponse(data, "Lấy thống kê theo tháng thành công"));
        }
    }
}
