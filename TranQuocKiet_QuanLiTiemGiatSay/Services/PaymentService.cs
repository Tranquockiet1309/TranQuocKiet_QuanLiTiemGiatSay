using Microsoft.EntityFrameworkCore;
using TranQuocKiet_QuanLiTiemGiatSay.Data;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Payments;
using TranQuocKiet_QuanLiTiemGiatSay.Models;

namespace TranQuocKiet_QuanLiTiemGiatSay.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICustomerService _customerService;

        public PaymentService(ApplicationDbContext context, ICustomerService customerService)
        {
            _context = context;
            _customerService = customerService;
        }

        public async Task<IEnumerable<PaymentResponse>> GetPaymentsByOrderIdAsync(long orderId)
        {
            return await _context.Payments
                .Include(p => p.Receiver)
                .Where(p => p.OrderId == orderId)
                .AsNoTracking()
                .OrderByDescending(p => p.PaymentTime)
                .Select(p => MapToResponse(p))
                .ToListAsync();
        }

        public async Task<PaymentResponse> CreateAsync(CreatePaymentRequest request, long userId)
        {
            var order = await _context.Orders.FindAsync(request.OrderId);
            if (order == null) throw new Exception("Order not found.");

            // Prevent overpayment - but allow idempotency if order is already paid
            decimal remaining = order.TotalAmount - order.PaidAmount;
            if (remaining <= 0)
            {
                // Đơn hàng đã được thanh toán đủ (có thể do callback bị gọi 2 lần)
                // Trả về payment gần nhất thay vì throw exception
                var lastPayment = await _context.Payments
                    .Where(p => p.OrderId == request.OrderId)
                    .OrderByDescending(p => p.PaymentTime)
                    .Include(p => p.Receiver)
                    .FirstOrDefaultAsync();
                if (lastPayment != null)
                    return MapToResponse(lastPayment);
                throw new Exception("Order is already fully paid.");
            }
            if (request.Amount > remaining)
            {
                throw new Exception($"Payment amount exceeds remaining balance ({remaining:N0}).");
            }

            var payment = new Payment
            {
                OrderId = request.OrderId,
                Amount = request.Amount,
                Method = Enum.TryParse<PaymentMethod>(request.Method, true, out var method) ? method : PaymentMethod.Cash,
                PaymentType = request.PaymentType.ToUpper(),
                ReceivedBy = userId,
                Note = request.Note,
                PaymentTime = DateTime.Now
            };

            order.PaidAmount += request.Amount;
            
            if (order.PaidAmount >= order.TotalAmount)
            {
                order.PaymentStatus = "PAID";
                // Trigger loyalty point update when fully paid
                await _customerService.UpdatePointsAsync(order.CustomerId, order.TotalAmount);
            }
            else if (order.PaidAmount > 0)
            {
                order.PaymentStatus = "PARTIAL";
            }

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Load receiver for mapping
            await _context.Entry(payment).Reference(p => p.Receiver).LoadAsync();

            return MapToResponse(payment);
        }

        private static PaymentResponse MapToResponse(Payment p)
        {
            return new PaymentResponse
            {
                PaymentId = p.PaymentId,
                OrderId = p.OrderId,
                PaymentTime = p.PaymentTime,
                Amount = p.Amount,
                Method = p.Method.ToString(),
                PaymentType = p.PaymentType,
                ReceivedBy = p.ReceivedBy,
                ReceiverName = p.Receiver?.FullName ?? "Unknown",
                Note = p.Note
            };
        }
    }
}
