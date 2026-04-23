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

        public async Task<List<MonthlyMetric>> GetMonthlySummaryAsync(int months)
        {
            var result = new List<MonthlyMetric>();
            var now = DateTime.Now;

            // Build month range (oldest → newest)
            var monthList = Enumerable.Range(0, months)
                .Select(i => now.AddMonths(-i))
                .Reverse()
                .ToList();

            // Orders grouped by month
            var startOfRange = new DateTime(monthList.First().Year, monthList.First().Month, 1);
            var ordersByMonth = await _context.Orders
                .Where(o => o.CreatedAt >= startOfRange)
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Total = g.Count(),
                    Completed = g.Count(o => o.Status == Models.OrderStatus.Completed || o.Status == Models.OrderStatus.Delivered)
                })
                .ToListAsync();

            // Revenue (payments) grouped by month
            var revenueByMonth = await _context.Payments
                .Where(p => p.PaymentTime >= startOfRange)
                .GroupBy(p => new { p.PaymentTime.Year, p.PaymentTime.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Amount = g.Sum(p => p.Amount)
                })
                .ToListAsync();

            var viMonths = new[] { "", "Th.1", "Th.2", "Th.3", "Th.4", "Th.5", "Th.6", "Th.7", "Th.8", "Th.9", "Th.10", "Th.11", "Th.12" };

            foreach (var m in monthList)
            {
                var ord = ordersByMonth.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month);
                var rev = revenueByMonth.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month);

                result.Add(new MonthlyMetric
                {
                    Year = m.Year,
                    Month = m.Month,
                    MonthLabel = $"{viMonths[m.Month]}/{m.Year}",
                    TotalOrders = ord?.Total ?? 0,
                    CompletedOrders = ord?.Completed ?? 0,
                    Revenue = rev?.Amount ?? 0
                });
            }

            return result;
        }
    }
}
