using JumiaProject.Models;

namespace JumiaProject.ViewModels
{
    public class DashboardViewModel
    {
        public List<Product> TopProducts { get; set; }
        public List<Product> PendingApprovals { get; set; }
        public List<Order> RecentOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalSellers { get; set; }
        public Dictionary<string, decimal> MonthlySales { get; set; }
        public Dictionary<string, int> CategoryDistribution { get; set; }
        public List<MostViewedProductViewModel> MostViewedProducts { get; set; }
        public Dictionary<int, Dictionary<string, decimal>> MultiYearMonthlySales { get; set; }
    }

    public class MostViewedProductViewModel
    {
        public Product Product { get; set; }
        public int ViewCount { get; set; }
    }

    public class MonthlySalesData
    {
        public string Month { get; set; }
        public decimal Amount { get; set; }
    }

    public class CategoryDistribution
    {
        public string Category { get; set; }
        public int Count { get; set; }
    }
}
