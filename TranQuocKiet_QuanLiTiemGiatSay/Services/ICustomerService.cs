using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Customers;

namespace TranQuocKiet_QuanLiTiemGiatSay.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerResponse>> GetAllAsync();
        Task<CustomerResponse?> GetByIdAsync(long id);
        Task<CustomerResponse> CreateAsync(CreateCustomerRequest request);
        Task<CustomerResponse?> UpdateAsync(long id, UpdateCustomerRequest request);
        Task<bool> DeleteAsync(long id);
        Task UpdatePointsAsync(long customerId, decimal orderAmount);
    }
}
