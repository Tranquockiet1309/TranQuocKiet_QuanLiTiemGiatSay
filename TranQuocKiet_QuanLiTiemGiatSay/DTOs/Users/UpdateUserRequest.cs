namespace TranQuocKiet_QuanLiTiemGiatSay.DTOs.Users
{
    public class UpdateUserRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Role { get; set; } = "STAFF";
        public bool IsActive { get; set; }
    }
}