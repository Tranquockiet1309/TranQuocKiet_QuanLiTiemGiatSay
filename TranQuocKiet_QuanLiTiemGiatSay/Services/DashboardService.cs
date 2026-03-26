using Microsoft.EntityFrameworkCore;
using TranQuocKiet_QuanLiTiemGiatSay.Data;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Dashboard;

namespace TranQuocKiet_QuanLiTiemGiatSay.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsResponse> GetStatsAsync(DateTime startDate, DateTime endDate)
        {
            var stats = new DashboardStatsResponse();

            // 1. Basic Stats (Overall in range)
            stats.TotalRevenue = await _context.Payments
                .Where(p => p.PaymentTime >= startDate && p.PaymentTime <= endDate)
                .SumAsync(p => p.Amount);

            stats.TotalExpenses = await _context.InventoryTransactions
                .Where(t => t.TxnType == "IN" && t.TxnDate >= startDate && t.TxnDate <= endDate)
                .SumAsync(t => t.Quantity * (t.UnitCost ?? 0));

            stats.TotalProfit = stats.TotalRevenue - stats.TotalExpenses;
            stats.TotalCustomers = await _context.Customers.CountAsync();

            // 2. Daily Metrics
            var dailyRevenue = await _context.Payments
                .Where(p => p.PaymentTime >= startDate && p.PaymentTime <= endDate)
                .GroupBy(p => p.PaymentTime.Date)
                .Select(g => new { Date = g.Key, Amount = g.Sum(x => x.Amount) })
                .ToListAsync();

            var dailyExpenses = await _context.InventoryTransactions
                .Where(t => t.TxnType == "IN" && t.TxnDate >= startDate && t.TxnDate <= endDate)
                .GroupBy(t => t.TxnDate.Date)
                .Select(g => new { Date = g.Key, Amount = g.Sum(x => x.Quantity * (x.UnitCost ?? 0)) })
                .ToListAsync();

            var dailyCustomers = await _context.Customers
                .Where(c => c.CreatedAt >= startDate && c.CreatedAt <= endDate)
                .GroupBy(c => c.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();

            // Combine into daily metrics
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                var rev = dailyRevenue.FirstOrDefault(x => x.Date == date)?.Amount ?? 0;
                var exp = dailyExpenses.FirstOrDefault(x => x.Date == date)?.Amount ?? 0;
                var cust = dailyCustomers.FirstOrDefault(x => x.Date == date)?.Count ?? 0;

                stats.DailyMetrics.Add(new DailyMetric
                {
                    Date = date,
                    Revenue = rev,
                    Expenses = exp,
                    Profit = rev - exp,
                    NewCustomers = cust
                });
            }

            // 3. Inventory Warnings (Aggregated current balance)
            var inventoryItems = await _context.InventoryTransactions
                .GroupBy(t => new { t.ItemName, t.Unit })
                .Select(g => new
                {
                    g.Key.ItemName,
                    g.Key.Unit,
                    Balance = g.Sum(x => x.TxnType == "IN" ? x.Quantity : -x.Quantity)
                })
                .Where(x => x.Balance < 10) // Threshold = 10
                .ToListAsync();

            stats.InventoryWarnings = inventoryItems.Select(x => new InventoryWarning
            {
                ItemName = x.ItemName,
                CurrentBalance = x.Balance,
                Unit = x.Unit
            }).ToList();

            return stats;
        }
    }
}
