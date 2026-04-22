using System;
using Microsoft.EntityFrameworkCore;
using TranQuocKiet_QuanLiTiemGiatSay.Data;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Orders;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Delivery;
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
                .Include(o => o.DeliveryProof)
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
                .Include(o => o.DeliveryProof)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            return order != null ? MapToResponse(order) : null;
        }

        public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, long userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var customerId = request.CustomerId;
                if (customerId == 0)
                {
                    var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
                    if (customer == null)
                    {
                        // Self-healing: Create customer record if missing for a CUSTOMER role user
                        var user = await _context.Users.FindAsync(userId);
                        if (user != null && user.Role == "CUSTOMER")
                        {
                            customer = new Customer
                            {
                                UserId = userId,
                                FullName = user.FullName,
                                Phone = user.Phone ?? ""
                            };
                            _context.Customers.Add(customer);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            throw new Exception("Không tìm thấy hồ sơ Khách hàng gắn với tài khoản này. Vui lòng liên hệ Admin.");
                        }
                    }
                    customerId = customer.CustomerId;
                }

                // If a customer is placing an order, they can't "receive" it themselves in the system record.
                // We'll set ReceivedBy to the first available Owner/Staff if the current user is a Customer.
                var currentUser = await _context.Users.FindAsync(userId);
                long receivedById = userId;
                if (currentUser != null && currentUser.Role == "CUSTOMER")
                {
                    var owner = await _context.Users.FirstOrDefaultAsync(u => u.Role == "OWNER" || u.Role == "STAFF");
                    if (owner != null) receivedById = owner.UserId;
                }

                if (request.Items == null || !request.Items.Any())
                {
                    throw new Exception("Đơn hàng phải có ít nhất một dịch vụ.");
                }

                var order = new Order
                {
                    OrderCode = $"ORD-{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(100, 999)}", // Added random to prevent collisions
                    CustomerId = customerId,
                    ReceivedBy = receivedById,
                    ReceiveMethod = request.ReceiveMethod ?? "AT_STORE",
                    Status = OrderStatus.Pending,
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
                    if (service == null) throw new Exception($"Dịch vụ (ID: {itemReq.ServiceId}) không tồn tại hoặc đã ngừng cung cấp.");

                    var lineAmount = service.UnitPrice * itemReq.Quantity;
                    subtotal += lineAmount;

                    order.OrderItems.Add(new OrderItem
                    {
                        ServiceId = itemReq.ServiceId,
                        Quantity = itemReq.Quantity,
                        UnitPrice = service.UnitPrice,
                        LineAmount = lineAmount,
                        ItemDescription = itemReq.ItemDescription ?? "",
                        ItemStatus = "PENDING"
                    });
                }

                order.SubtotalAmount = subtotal;
                order.TotalAmount = subtotal - request.DiscountAmount + request.DeliveryFee;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Reload navigation properties
                await _context.Entry(order).Reference(o => o.Customer).LoadAsync();
                foreach (var item in order.OrderItems)
                {
                    await _context.Entry(item).Reference(oi => oi.Service).LoadAsync();
                }

                return MapToResponse(order);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var fullMessage = $"Lỗi tạo đơn hàng: {ex.Message}. {(ex.InnerException != null ? "Chi tiết: " + ex.InnerException.Message : "")}";
                Console.WriteLine(fullMessage);
                Console.WriteLine(ex.StackTrace);
                throw new Exception(fullMessage, ex);
            }
        }

        public async Task<OrderResponse?> UpdateStatusAsync(long id, string status)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Service)
                .Include(o => o.DeliveryProof)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return null;

            if (Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
            {
                order.Status = orderStatus;
            }
            else
            {
                throw new ArgumentException($"Invalid order status: {status}");
            }

            if (order.Status == OrderStatus.Completed)
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
                Status = o.Status.ToString(),
                ReceivedAt = o.ReceivedAt,
                PromisedAt = o.PromisedAt,
                TotalAmount = o.TotalAmount,
                PaidAmount = o.PaidAmount,
                PaymentStatus = o.PaymentStatus,
                DeliveryProof = o.DeliveryProof != null ? new DeliveryProofResponse
                {
                    Id = o.DeliveryProof.Id,
                    OrderId = o.DeliveryProof.OrderId,
                    ImageUrl = o.DeliveryProof.ImageUrl,
                    CreatedAt = o.DeliveryProof.CreatedAt
                } : null,
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
