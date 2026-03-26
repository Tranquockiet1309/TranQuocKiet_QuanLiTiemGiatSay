using Microsoft.EntityFrameworkCore;
using TranQuocKiet_QuanLiTiemGiatSay.Constants;
using TranQuocKiet_QuanLiTiemGiatSay.Models;
using TranQuocKiet_QuanLiTiemGiatSay.Services;

namespace TranQuocKiet_QuanLiTiemGiatSay.Data
{
    public static class DbSeeder
    {
        public static async Task SeedOwnerAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();

            var hasAnyUser = await context.Users.AnyAsync();
            if (hasAnyUser) return;

                var owner = new User
                {
                FullName = "Chủ tiệm",
                    Phone = "0900000000",
                    Username = "owner",
                    PasswordHash = passwordService.HashPassword("123456"),
                    Role = Roles.OWNER,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

            context.Users.Add(owner);
                await context.SaveChangesAsync();
            }
        }
    }