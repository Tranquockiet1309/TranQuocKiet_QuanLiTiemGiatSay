namespace TranQuocKiet_QuanLiTiemGiatSay.DTOs.Users
{
    public class UserResponse
    {
        public long UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}