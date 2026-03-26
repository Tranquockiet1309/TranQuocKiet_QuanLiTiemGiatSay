using Microsoft.EntityFrameworkCore;
using TranQuocKiet_QuanLiTiemGiatSay.Data;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Orders;
using TranQuocKiet_QuanLiTiemGiatSay.Models;

namespace TranQuocKiet_QuanLiTiemGiatSay.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICustomerService _customerService;

        public OrderService(ApplicationDbContext context, ICustomerService customerService)
        {
            _context = context;
            _customerService = customerService;
        }

        public async Task<IEnumerable<OrderResponse>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Service)
                .AsNoTracking()
                .OrderByDescending(o => o.ReceivedAt)
                .Select(o => MapToResponse(o))
                .ToListAsync();
        }

        public async Task<OrderResponse?> GetByIdAsync(long id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Service)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            return order != null ? MapToResponse(order) : null;
        }

        public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, long userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    OrderCode = $"ORD-{DateTime.Now:yyyyMMddHHmmss}",
                    CustomerId = request.CustomerId,
                    ReceivedBy = userId,
                    ReceiveMethod = request.ReceiveMethod,
                    Status = "PENDING",
                    ReceivedAt = DateTime.Now,
                    PromisedAt = request.PromisedAt,
                    DiscountAmount = request.DiscountAmount,
                    DeliveryFee = request.DeliveryFee,
                    OrderNote = request.OrderNote,
                    PaymentStatus = "UNPAID"
                };

                decimal subtotal = 0;

                foreach (var itemReq in request.Items)
                {
                    var service = await _context.Services.FindAsync(itemReq.ServiceId);
                    if (service == null) throw new Exception($"Service with ID {itemReq.ServiceId} not found.");

                    var lineAmount = service.UnitPrice * itemReq.Quantity;
                    subtotal += lineAmount;

                    order.OrderItems.Add(new OrderItem
                    {
                        ServiceId = itemReq.ServiceId,
                        Quantity = itemReq.Quantity,
                        UnitPrice = service.UnitPrice,
                        LineAmount = lineAmount,
                        ItemDescription = itemReq.ItemDescription,
                        ItemStatus = "PENDING"
                    });
                }

                order.SubtotalAmount = subtotal;
                order.TotalAmount = subtotal - request.DiscountAmount + request.DeliveryFee;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Reload to get navigation properties for mapping
                await _context.Entry(order).Reference(o => o.Customer).LoadAsync();
                foreach (var item in order.OrderItems)
                {
                    await _context.Entry(item).Reference(oi => oi.Service).LoadAsync();
                }

                return MapToResponse(order);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<OrderResponse?> UpdateStatusAsync(long id, string status)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Service)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return null;

            order.Status = status.ToUpper();
            if (order.Status == "COMPLETED")
            {
                order.CompletedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return MapToResponse(order);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        private static OrderResponse MapToResponse(Order o)
        {
            return new OrderResponse
            {
                OrderId = o.OrderId,
                OrderCode = o.OrderCode,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer?.FullName ?? "Unknown",
                Status = o.Status,
                ReceivedAt = o.ReceivedAt,
                PromisedAt = o.PromisedAt,
                TotalAmount = o.TotalAmount,
                PaidAmount = o.PaidAmount,
                PaymentStatus = o.PaymentStatus,
                Items = o.OrderItems.Select(oi => new OrderItemResponse
                {
                    OrderItemId = oi.OrderItemId,
                    ServiceName = oi.Service?.ServiceName ?? "Unknown",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    LineAmount = oi.LineAmount,
                    ItemStatus = oi.ItemStatus
                }).ToList()
            };
        }
    }
}
