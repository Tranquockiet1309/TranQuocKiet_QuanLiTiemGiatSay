using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranQuocKiet_QuanLiTiemGiatSay.Constants;
using TranQuocKiet_QuanLiTiemGiatSay.Data;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Users;
using TranQuocKiet_QuanLiTiemGiatSay.Models;
using TranQuocKiet_QuanLiTiemGiatSay.Services;

namespace TranQuocKiet_QuanLiTiemGiatSay.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    [Authorize(Roles = Roles.OWNER)]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService;

        public UsersController(ApplicationDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .OrderByDescending(x => x.UserId)
                .Select(x => new UserResponse
                {
                    UserId = x.UserId,
                    FullName = x.FullName,
                    Phone = x.Phone,
                    Username = x.Username,
                    Role = x.Role,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                message = "Lấy danh sách người dùng thành công",
                data = users
            });
        }

        [HttpGet("role/{role}")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            var users = await _context.Users
                .Where(x => x.Role == role)
                .OrderByDescending(x => x.UserId)
                .Select(x => new UserResponse
                {
                    UserId = x.UserId,
                    FullName = x.FullName,
                    Phone = x.Phone,
                    Username = x.Username,
                    Role = x.Role,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                message = $"Lấy danh sách người dùng có role {role} thành công",
                data = users
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "FullName, Username, Password là bắt buộc"
                });
            }

            var existedUsername = await _context.Users.AnyAsync(x => x.Username == request.Username);
            if (existedUsername)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Username đã tồn tại"
                });
            }

            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                var existedPhone = await _context.Users.AnyAsync(x => x.Phone == request.Phone);
                if (existedPhone)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Số điện thoại đã tồn tại"
                    });
                }
            }

            if (request.Role != Roles.OWNER && request.Role != Roles.STAFF && request.Role != Roles.SHIPPER)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Role không hợp lệ"
                });
            }

            var user = new User
            {
                FullName = request.FullName.Trim(),
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                Username = request.Username.Trim(),
                PasswordHash = _passwordService.HashPassword(request.Password),
                Role = request.Role,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var response = new UserResponse
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Phone = user.Phone,
                Username = user.Username,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };

            return Ok(new
            {
                success = true,
                message = "Tạo người dùng thành công",
                data = response
            });
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] UpdateUserRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == id);
            if (user == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy người dùng"
                });
            }

            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                var existedPhone = await _context.Users
                    .AnyAsync(x => x.Phone == request.Phone && x.UserId != id);

                if (existedPhone)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Số điện thoại đã tồn tại"
                    });
                }
            }

            if (request.Role != Roles.OWNER && request.Role != Roles.STAFF && request.Role != Roles.SHIPPER)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Role không hợp lệ"
                });
            }

            user.FullName = request.FullName.Trim();
            user.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
            user.Role = request.Role;
            user.IsActive = request.IsActive;

            await _context.SaveChangesAsync();

            var response = new UserResponse
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Phone = user.Phone,
                Username = user.Username,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };

            return Ok(new
            {
                success = true,
                message = "Cập nhật người dùng thành công",
                data = response
            });
        }
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetUserById(long id)
        {
            var user = await _context.Users
                .Where(x => x.UserId == id)
                .Select(x => new UserResponse
                {
                    UserId = x.UserId,
                    FullName = x.FullName,
                    Phone = x.Phone,
                    Username = x.Username,
                    Role = x.Role,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy người dùng"
                });
            }

            return Ok(new
            {
                success = true,
                data = user
            });
        }
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy người dùng" });
            }

            // check if user is the last owner
            if (user.Role == Roles.OWNER)
            {
                var ownerCount = await _context.Users.CountAsync(x => x.Role == Roles.OWNER && x.IsActive);
                if (ownerCount <= 1)
                {
                    return BadRequest(new { success = false, message = "Không thể xóa chủ sở hữu cuối cùng" });
                }
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Xóa người dùng thành công" });
        }
    }
}