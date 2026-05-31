using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sklep.Core.Models;
using Sklep.Core.Models.DTOs;
using Sklep.Infrastructure.Data;

namespace Sklep.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductVariantsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductVariantsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index(int productId)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product == null) return NotFound();

        ViewBag.ProductName = product.Name;
        ViewBag.ProductId = product.Id;

        var variants = await _context.ProductVariants
            .Include(v => v.Size)
            .Where(v => v.ProductId == productId)
            .ToListAsync();

        return View(variants);
    }
    
    public async Task<IActionResult> Create(int productId)
    {
        ViewBag.ProductId = productId;
        ViewBag.Sizes = new SelectList(await _context.Sizes.ToListAsync(), "Id", "Name");
        
        var dto = new ProductVariantDto { ProductId = productId };
        return View(dto);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductVariantDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ProductId = dto.ProductId;
            ViewBag.Sizes = new SelectList(await _context.Sizes.ToListAsync(), "Id", "Name", dto.SizeId);
            return View(dto);
        }
        
        var variant = new ProductVariant
        {
            ProductId = dto.ProductId,
            SizeId = dto.SizeId,
            StockQuantity = dto.StockQuantity,
            Color = dto.Color ?? string.Empty
        };

        _context.ProductVariants.Add(variant);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { productId = dto.ProductId });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var variant = await _context.ProductVariants.FindAsync(id);
        if (variant == null) return NotFound();

        int productId = variant.ProductId;
        
        _context.ProductVariants.Remove(variant);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { productId = productId });
    }
}