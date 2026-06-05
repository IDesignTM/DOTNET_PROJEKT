using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sklep.Core.Models;
using Sklep.Infrastructure.Data;

namespace Sklep.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DiscountCodesController : Controller
{
    private readonly ApplicationDbContext _context;

    public DiscountCodesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var codes = await _context.DiscountCodes.ToListAsync();
        return View(codes);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DiscountCode model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _context.DiscountCodes.Add(model);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var code = await _context.DiscountCodes.FindAsync(id);
        if (code == null) return NotFound();

        return View(code);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(DiscountCode model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _context.DiscountCodes.Update(model);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var code = await _context.DiscountCodes.FindAsync(id);
        if (code == null) return NotFound();

        _context.DiscountCodes.Remove(code);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}