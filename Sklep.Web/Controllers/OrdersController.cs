using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sklep.Core.Models;
using Sklep.Infrastructure.Data;
using Sklep.Web.ViewModels;
using System.Security.Claims;

namespace Sklep.Web.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Checkout()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var cartJson = HttpContext.Session.GetString("Cart");

        if (string.IsNullOrEmpty(cartJson))
            return RedirectToAction("Index", "Cart");

        var cart = JsonConvert.DeserializeObject<List<Product>>(cartJson);

        if (cart == null || !cart.Any())
            return RedirectToAction("Index", "Cart");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var order = new Order
        {
            UserId = userId!,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Address = model.Address,
            City = model.City,
            PostalCode = model.PostalCode,
            TotalPrice = cart.Sum(p => p.Price)
        };

        foreach (var product in cart)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = 1,
                UnitPrice = product.Price
            });
        }

        _context.Orders.Add(order);

        await _context.SaveChangesAsync();

        HttpContext.Session.Remove("Cart");

        return RedirectToAction("Success", new { id = order.Id });
    }

    public IActionResult Success(int id)
    {
        return View(model: id);
    }

    public async Task<IActionResult> MyOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o =>
                o.Id == id &&
                o.UserId == userId);

        if (order == null)
            return NotFound();

        return View(order);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AllOrders()
    {
        var orders = await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return View(orders);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Manage(int id)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return NotFound();

        return View(order);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Manage(int id, string status)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
            return NotFound();

        order.Status = status;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(AllOrders));
    }
}