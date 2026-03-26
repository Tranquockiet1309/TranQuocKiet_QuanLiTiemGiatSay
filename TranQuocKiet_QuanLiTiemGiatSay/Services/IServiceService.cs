using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Services;

namespace TranQuocKiet_QuanLiTiemGiatSay.Services
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceResponse>> GetAllAsync(bool onlyActive = false);
        Task<ServiceResponse?> GetByIdAsync(long id);
        Task<ServiceResponse> CreateAsync(CreateServiceRequest request);
        Task<ServiceResponse?> UpdateAsync(long id, UpdateServiceRequest request);
        Task<bool> DeleteAsync(long id);
    }
}
