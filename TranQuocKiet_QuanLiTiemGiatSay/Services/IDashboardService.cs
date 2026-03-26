using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Dashboard;

namespace TranQuocKiet_QuanLiTiemGiatSay.Services
{
    public interface IDashboardService
    {
        Task<DashboardStatsResponse> GetStatsAsync(DateTime startDate, DateTime endDate);
    }
}
