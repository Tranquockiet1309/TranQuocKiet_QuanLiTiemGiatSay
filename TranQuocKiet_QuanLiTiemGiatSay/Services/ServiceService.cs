using Microsoft.EntityFrameworkCore;
using TranQuocKiet_QuanLiTiemGiatSay.Data;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Services;
using TranQuocKiet_QuanLiTiemGiatSay.Models;

namespace TranQuocKiet_QuanLiTiemGiatSay.Services
{
    public class ServiceService : IServiceService
    {
        private readonly ApplicationDbContext _context;

        public ServiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceResponse>> GetAllAsync(bool onlyActive = false)
        {
            var query = _context.Services.AsNoTracking();
            if (onlyActive) query = query.Where(s => s.IsActive);

            return await query
                .OrderBy(s => s.ServiceName)
                .Select(s => MapToResponse(s))
                .ToListAsync();
        }

        public async Task<ServiceResponse?> GetByIdAsync(long id)
        {
            var service = await _context.Services.FindAsync(id);
            return service != null ? MapToResponse(service) : null;
        }

        public async Task<ServiceResponse> CreateAsync(CreateServiceRequest request)
        {
            var service = new Service
            {
                ServiceName = request.ServiceName.Trim(),
                Unit = request.Unit.Trim(),
                UnitPrice = request.UnitPrice,
                EstimatedHours = request.EstimatedHours,
                IsActive = true
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return MapToResponse(service);
        }

        public async Task<ServiceResponse?> UpdateAsync(long id, UpdateServiceRequest request)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return null;

            service.ServiceName = request.ServiceName.Trim();
            service.Unit = request.Unit.Trim();
            service.UnitPrice = request.UnitPrice;
            service.EstimatedHours = request.EstimatedHours;
            service.IsActive = request.IsActive;

            await _context.SaveChangesAsync();
            return MapToResponse(service);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return false;

            // Check if service is used in any order items
            bool isUsed = await _context.OrderItems.AnyAsync(oi => oi.ServiceId == id);
            if (isUsed)
            {
                // Soft delete by deactivating if it's already used
                service.IsActive = false;
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        private static ServiceResponse MapToResponse(Service s)
        {
            return new ServiceResponse
            {
                ServiceId = s.ServiceId,
                ServiceName = s.ServiceName,
                Unit = s.Unit,
                UnitPrice = s.UnitPrice,
                EstimatedHours = s.EstimatedHours,
                IsActive = s.IsActive
            };
        }
    }
}
