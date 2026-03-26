namespace TranQuocKiet_QuanLiTiemGiatSay.DTOs.Dashboard
{
    public class DashboardStatsResponse
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalProfit { get; set; }
        public int TotalCustomers { get; set; }
        public List<DailyMetric> DailyMetrics { get; set; } = new();
        public List<InventoryWarning> InventoryWarnings { get; set; } = new();
    }

    public class DailyMetric
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public decimal Profit { get; set; }
        public int NewCustomers { get; set; }
    }

    public class InventoryWarning
    {
        public string ItemName { get; set; } = string.Empty;
        public decimal CurrentBalance { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
}
