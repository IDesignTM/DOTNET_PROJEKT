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
                .ToListAsync()
        };

        return View(model);
    }
}