using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Inventory;

namespace TranQuocKiet_QuanLiTiemGiatSay.Services
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryTxnResponse>> GetAllAsync();
        Task<InventoryTxnResponse> CreateAsync(CreateInventoryTxnRequest request, long userId);
    }
}
