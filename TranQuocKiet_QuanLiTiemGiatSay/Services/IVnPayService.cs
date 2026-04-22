using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Payments;

namespace TranQuocKiet_QuanLiTiemGiatSay.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPayRequest request);
        VnPayResponse PaymentExecute(IQueryCollection collections);
    }
}
