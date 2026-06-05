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

        var cart = JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

        if (cart == null || !cart.Any())
            return RedirectToAction("Index", "Cart");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        decimal total = cart.Sum(i => i.Product.Price * i.Quantity);

        DiscountCode? discount = null;

        if (!string.IsNullOrWhiteSpace(model.DiscountCode))
        {
            discount = await _context.DiscountCodes
                .FirstOrDefaultAsync(x =>
                    x.Code == model.DiscountCode &&
                    x.IsActive &&
                    x.ExpirationDate > DateTime.Now);

            if (discount != null)
            {
                total -= total * (discount.DiscountPercent / 100);
            }
        }

        var order = new Order
        {
            UserId = userId!,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Address = model.Address,
            City = model.City,
            PostalCode = model.PostalCode,
            TotalPrice = total
        };

        foreach (var item in cart)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = item.Product.Id,
                Quantity = item.Quantity,
                UnitPrice = item.Product.Price
            });
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var payment = new Payment
        {
            OrderId = order.Id,
            Amount = order.TotalPrice,
            Method = "Płatność online",
            Status = "Oczekująca"
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        if (discount != null)
        {
            var usage = new DiscountCodeUsage
            {
                DiscountCodeId = discount.Id,
                OrderId = order.Id,
                UserId = userId!
            };

            _context.DiscountCodeUsages.Add(usage);
            await _context.SaveChangesAsync();
        }

        HttpContext.Session.Remove("Cart");

        return RedirectToAction(
            "Pay",
            "Payment",
            new { orderId = order.Id }
        );
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
            .Include(o => o.Payment)
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

                page.Header().Text($"FAKTURA nr FV/{DateTime.Now.Year}/{order.Id}")
                    .FontSize(22)
                    .Bold()
                    .AlignCenter();

                page.Content().Column(col =>
                {
                    col.Item().PaddingTop(20);

                    col.Item().Text("SKLEP ODZIEŻOWY")
                        .Bold()
                        .FontSize(16);

                    col.Item().Text("ul. Wiejska 1");
                    col.Item().Text("15-351 Białystok");
                    col.Item().Text("NIP: 8239401642");
                    col.Item().Text("tel. 764 329 443");
                    col.Item().Text("e-mail: sklep@wp.pl");

                    col.Item().PaddingTop(15);

                    col.Item().Text($"Data wystawienia: {order.CreatedAt:yyyy-MM-dd}");

                    col.Item().PaddingTop(15);

                    col.Item().Text("NABYWCA")
                        .Bold();

                    col.Item().Text($"{order.FirstName} {order.LastName}");
                    col.Item().Text(order.Address);
                    col.Item().Text($"{order.PostalCode} {order.City}");

                    col.Item().PaddingTop(20);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Border(1).Padding(5).Text("Produkt").Bold();
                            header.Cell().Border(1).Padding(5).Text("Ilość").Bold();
                            header.Cell().Border(1).Padding(5).Text("Cena/szt.").Bold();
                            header.Cell().Border(1).Padding(5).Text("Wartość").Bold();
                        });

                        foreach (var item in order.Items)
                        {
                            table.Cell().Border(1).Padding(5)
                                .Text(item.Product.Name);

                            table.Cell().Border(1).Padding(5)
                                .Text(item.Quantity.ToString());

                            table.Cell().Border(1).Padding(5)
                                .Text(item.UnitPrice.ToString("C"));

                            table.Cell().Border(1).Padding(5)
                                .Text((item.Quantity * item.UnitPrice).ToString("C"));
                        }
                    });

                    col.Item().PaddingTop(20);

                    col.Item()
                        .AlignRight()
                        .Text($"Razem do zapłaty: {order.TotalPrice:C}")
                        .Bold()
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