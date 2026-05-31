using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sklep.Core.Models;
using Sklep.Infrastructure.Data;

namespace Sklep.Web.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public UsersController(
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public IActionResult Index()
    {
        var users = _userManager.Users.ToList();

        return View(users);
    }

    public async Task<IActionResult> Manage(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        ViewBag.IsAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> MakeAdmin(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        await _userManager.AddToRoleAsync(user, "Admin");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveAdmin(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        var currentUserId = _userManager.GetUserId(User);

        if (user.Id == currentUserId)
        {
            TempData["Error"] = "Nie możesz odebrać sobie roli administratora.";
            return RedirectToAction(nameof(Index));
        }

        await _userManager.RemoveFromRoleAsync(user, "Admin");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var currentUserId = _userManager.GetUserId(User);

        // zabezpieczenie przed usunięciem samego siebie
        if (id == currentUserId)
        {
            return RedirectToAction(nameof(Index));
        }

        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        await _userManager.DeleteAsync(user);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        var orders = await _context.Orders
            .Where(o => o.UserId == id)
            .ToListAsync();

        ViewBag.OrdersCount = orders.Count();
        ViewBag.TotalSpent = orders.Sum(o => o.TotalPrice);

        return View(user);
    }
}



