using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Import;

namespace TranQuocKiet_QuanLiTiemGiatSay.Services
{
    public interface IImportService
    {
        Task<ImportResult> ImportOrdersAsync(BatchImportOrderRequest request, long userId);
    }
}
