using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Payments;

namespace TranQuocKiet_QuanLiTiemGiatSay.Services
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentResponse>> GetPaymentsByOrderIdAsync(long orderId);
        Task<PaymentResponse> CreateAsync(CreatePaymentRequest request, long userId);
    }
}
