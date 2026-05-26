using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sklep.Core.Models;
using Sklep.Infrastructure.Data;

namespace Sklep.Web.Controllers;

[Authorize]
public class WishlistController : Controller
{
    private readonly ApplicationDbContext _context;

    public WishlistController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var wishlist = await _context.Wishlists
            .Include(w => w.Items)
            .ThenInclude(i => i.Product)
            .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wishlist == null)
        {
            wishlist = new Wishlist { UserId = userId! };
        }

        return View(wishlist);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToWishlist(int productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();
        
        var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
        if (!productExists) return NotFound();
        
        var wishlist = await _context.Wishlists
            .Include(w => w.Items)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wishlist == null)
        {
            wishlist = new Wishlist { UserId = userId };
            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();
        }
        
        var itemExists = wishlist.Items.Any(i => i.ProductId == productId);
        if (!itemExists)
        {
            var newItem = new WishlistItem
            {
                WishlistId = wishlist.Id,
                ProductId = productId
            };
            _context.WishlistItems.Add(newItem);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Details", "Products", new { id = productId });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFromWishlist(int itemId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var wishlistItem = await _context.WishlistItems
            .Include(i => i.Wishlist)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.Wishlist!.UserId == userId);

        if (wishlistItem != null)
        {
            _context.WishlistItems.Remove(wishlistItem);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}