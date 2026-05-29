using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
}