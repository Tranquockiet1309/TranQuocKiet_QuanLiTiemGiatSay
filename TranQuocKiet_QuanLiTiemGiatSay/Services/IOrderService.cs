using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Orders;

namespace TranQuocKiet_QuanLiTiemGiatSay.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponse>> GetAllAsync();
        Task<OrderResponse?> GetByIdAsync(long id);
        Task<OrderResponse> CreateAsync(CreateOrderRequest request, long userId);
        Task<OrderResponse?> UpdateStatusAsync(long id, string status);
        Task<bool> DeleteAsync(long id);
    }
}
