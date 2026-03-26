namespace TranQuocKiet_QuanLiTiemGiatSay.DTOs.Auth
{
    public class RegisterRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
