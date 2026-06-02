using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sklep.Core.Models;
using Sklep.Infrastructure.Data;
using Sklep.Web.ViewModels;
using System.Security.Claims;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminDetails(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return NotFound();

        return View(order);
    }

    public async Task<IActionResult> GeneratePdf(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return NotFound();

        if (!User.IsInRole("Admin"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (order.UserId != userId)
                return Forbid();
        }

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Content().Column(col =>
                {
                    col.Item().Text($"Faktura nr {order.Id}")
                        .FontSize(20);

                    col.Item().Text($"Data: {order.CreatedAt:yyyy-MM-dd}");

                    col.Item().Text($"Klient: {order.FirstName} {order.LastName}");

                    col.Item().Text($"Adres: {order.Address}");

                    col.Item().Text($"{order.PostalCode} {order.City}");

                    col.Item().PaddingTop(20);

                    foreach (var item in order.Items)
                    {
                        col.Item().Text(
                            $"{item.Product.Name} x {item.Quantity} - {item.UnitPrice:C}");
                    }

                    col.Item().PaddingTop(20);

                    col.Item().Text(
                        $"Razem: {order.TotalPrice:C}")
                        .FontSize(16);
                });
            });
        });

        var bytes = pdf.GeneratePdf();

        return File(
            bytes,
            "application/pdf",
            $"Faktura_{order.Id}.pdf");
    }

}