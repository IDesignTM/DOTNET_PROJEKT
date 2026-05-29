using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Sklep.Core.Interfaces;

namespace Sklep.Web.Controllers;

public class CheckoutController : Controller
{
    private readonly IOrderService _orderService;

    public CheckoutController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> BuyNow(int productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var orderId = await _orderService.CreateOrderAsync(userId!, productId);

        return RedirectToAction("Success", new { id = orderId });
    }

    public IActionResult Success(int id)
    {
        ViewBag.OrderId = id;

        return View();
    }
}