using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TranQuocKiet_QuanLiTiemGiatSay.Data;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Auth;
using TranQuocKiet_QuanLiTiemGiatSay.Models;
using TranQuocKiet_QuanLiTiemGiatSay.Services;

namespace TranQuocKiet_QuanLiTiemGiatSay.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(
            ApplicationDbContext context,
            IPasswordService passwordService,
            IJwtTokenService jwtTokenService)
        {
            _context = context;
            _passwordService = passwordService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var usernameOrPhone = request.UsernameOrPhone.Trim();

            var user = await _context.Users
                .Include(x => x.Customer)
                .Include(x => x.Shipper)
                .FirstOrDefaultAsync(x =>
                    x.Username == usernameOrPhone || x.Phone == usernameOrPhone);

            if (user == null)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Tài khoản không tồn tại"
                });
            }

            if (!user.IsActive)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Tài khoản đã bị khóa"
                });
            }

            var isValidPassword = _passwordService.VerifyPassword(request.Password, user.PasswordHash);
            if (!isValidPassword)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Sai mật khẩu"
                });
            }

            var token = _jwtTokenService.GenerateToken(user);

            var response = new LoginResponse
            {
                Token = token,
                UserId = user.UserId,
                FullName = user.FullName,
                Username = user.Username,
                Role = user.Role,
                CustomerId = user.Customer?.CustomerId,
                ShipperId = user.Shipper?.ShipperId
            };

            return Ok(new
            {
                success = true,
                message = "Đăng nhập thành công",
                data = response
            });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { success = false, message = "Họ tên, tên đăng nhập và mật khẩu là bắt buộc" });
            }

            // check username
            var existedUser = await _context.Users
                .AnyAsync(x => x.Username == request.Username);

            if (existedUser)
            {
                return BadRequest(new { success = false, message = "Tên đăng nhập đã tồn tại" });
            }

            // check phone (nên có)
            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                var existedPhone = await _context.Users
                    .AnyAsync(x => x.Phone == request.Phone);

                if (existedPhone)
                {
                    return BadRequest(new { success = false, message = "Số điện thoại đã tồn tại" });
                }
            }

            // 🔥 1. Tạo USER (CUSTOMER)
            var user = new User
            {
                FullName = request.FullName.Trim(),
                Username = request.Username.Trim(),
                Phone = request.Phone,
                PasswordHash = _passwordService.HashPassword(request.Password),
                Role = "CUSTOMER", // ✅ FIX QUAN TRỌNG
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // để lấy UserId

            // 🔥 2. Tạo CUSTOMER và link
            var customer = new Customer
            {
                FullName = user.FullName,
                Phone = user.Phone ?? "",
                UserId = user.UserId // 🔗 LINK
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Đăng ký tài khoản thành công"
            });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Token không hợp lệ"
                });
            }

            var userId = long.Parse(userIdClaim);

            var user = await _context.Users
                .Include(x => x.Customer)
                .Include(x => x.Shipper)
                .Where(x => x.UserId == userId)
                .Select(x => new
                {
                    x.UserId,
                    x.FullName,
                    x.Phone,
                    x.Username,
                    x.Role,
                    x.IsActive,
                    x.CreatedAt,
                    CustomerId = x.Customer != null ? (long?)x.Customer.CustomerId : null,
                    ShipperId = x.Shipper != null ? (long?)x.Shipper.ShipperId : null,
                    Address = x.Customer != null ? x.Customer.Address : null
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
                message = "Lấy thông tin người dùng thành công",
                data = user
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok(new
            {
                success = true,
                message = "Đăng xuất thành công"
            });
        }
    }
}