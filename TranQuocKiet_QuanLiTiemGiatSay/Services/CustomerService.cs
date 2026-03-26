using Microsoft.EntityFrameworkCore;
using TranQuocKiet_QuanLiTiemGiatSay.Data;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Customers;
using TranQuocKiet_QuanLiTiemGiatSay.Models;

namespace TranQuocKiet_QuanLiTiemGiatSay.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerResponse>> GetAllAsync()
        {
            return await _context.Customers
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => MapToResponse(c))
                .ToListAsync();
        }

        public async Task<CustomerResponse?> GetByIdAsync(long id)
        {
            var customer = await _context.Customers.FindAsync(id);
            return customer != null ? MapToResponse(customer) : null;
        }

        public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request)
        {
            var customer = new Customer
            {
                FullName = request.FullName.Trim(),
                Phone = request.Phone.Trim(),
                Address = request.Address?.Trim(),
                CustomerNote = request.CustomerNote?.Trim(),
                CreatedAt = DateTime.Now
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return MapToResponse(customer);
        }

        public async Task<CustomerResponse?> UpdateAsync(long id, UpdateCustomerRequest request)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return null;

            customer.FullName = request.FullName.Trim();
            customer.Phone = request.Phone.Trim();
            customer.Address = request.Address?.Trim();
            customer.CustomerNote = request.CustomerNote?.Trim();

            await _context.SaveChangesAsync();

            return MapToResponse(customer);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return false;

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdatePointsAsync(long customerId, decimal orderAmount)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer != null)
            {
                customer.TotalSpent += orderAmount;
                // Logic: 10,000 VND = 1 Point
                customer.PointBalance += (int)(orderAmount / 10000);
                await _context.SaveChangesAsync();
            }
        }

        private static CustomerResponse MapToResponse(Customer c)
        {
            return new CustomerResponse
            {
                CustomerId = c.CustomerId,
                FullName = c.FullName,
                Phone = c.Phone,
                Address = c.Address,
                CustomerNote = c.CustomerNote,
                TotalSpent = c.TotalSpent,
                PointBalance = c.PointBalance,
                CreatedAt = c.CreatedAt
            };
        }
    }
}
