using TranQuocKiet_QuanLiTiemGiatSay.Models;

namespace TranQuocKiet_QuanLiTiemGiatSay.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}