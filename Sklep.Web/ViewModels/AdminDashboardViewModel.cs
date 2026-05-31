using Sklep.Core.Models;

namespace Sklep.Web.ViewModels;

public class AdminDashboardViewModel
{
    public int ProductsCount { get; set; }

    public int CategoriesCount { get; set; }

    public int UsersCount { get; set; }

    public int OrdersCount { get; set; }

    public decimal TotalRevenue { get; set; }
    public List<Order> LatestOrders { get; set; } = new();
}