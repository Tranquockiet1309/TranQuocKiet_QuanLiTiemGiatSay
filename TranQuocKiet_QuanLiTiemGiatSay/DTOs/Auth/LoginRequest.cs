namespace TranQuocKiet_QuanLiTiemGiatSay.DTOs.Auth
{
    public class LoginRequest
    {
        public string UsernameOrPhone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}