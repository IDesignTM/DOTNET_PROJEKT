using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sklep.Core.Models;
using Sklep.Infrastructure.Data;
using Sklep.Web.ViewModels;

namespace Sklep.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var topDiscountCodes = await _context.DiscountCodeUsages
            .Include(x => x.DiscountCode)
            .GroupBy(x => x.DiscountCode.Code)
            .Select(g => new TopDiscountCodeViewModel
            {
                Code = g.Key,
                UsageCount = g.Count()
            })
            .OrderByDescending(x => x.UsageCount)
            .Take(3)
            .ToListAsync();

        var topViewedProducts = await _context.ProductViewHistories
            .Include(x => x.Product)
            .GroupBy(x => x.Product.Name)
            .Select(g => new TopViewedProductViewModel
            {
                ProductName = g.Key,
                ViewsCount = g.Count()
            })
            .OrderByDescending(x => x.ViewsCount)
            .Take(3)
            .ToListAsync();

        var latestLogins = await _context.LoginHistories
            .Include(x => x.User)
            .Where(x => x.Success)
            .OrderByDescending(x => x.LoginTime)
            .Take(3)
            .ToListAsync();

        var model = new AdminDashboardViewModel
        {
            ProductsCount = await _context.Products.CountAsync(),

            CategoriesCount = await _context.Categories.CountAsync(),

            UsersCount = await _userManager.Users.CountAsync(),

            OrdersCount = await _context.Orders.CountAsync(),

            TotalRevenue = await _context.Orders.SumAsync(o => o.TotalPrice),

            LatestOrders = await _context.Orders
                .OrderByDescending(o => o.Id)
                .Take(5)
                .ToListAsync(),

            TopDiscountCodes = topDiscountCodes,

            TopViewedProducts = topViewedProducts,

            LatestLogins = latestLogins
        };

        return View(model);
    }
}