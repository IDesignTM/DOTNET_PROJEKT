using Microsoft.AspNetCore.Mvc;
using Sklep.Core.Interfaces;
using Sklep.Core.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Sklep.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sklep.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly IBaseRepository<Category> _categoryRepository;
    private readonly ICurrencyService _currencyService;

    public ProductsController(
        IProductRepository productRepository, 
        IBaseRepository<Category> categoryRepository,
        ICurrencyService currencyService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _currencyService = currencyService;
    }
    
    public async Task<IActionResult> Index(string? categoryName, string? searchString, int? sizeId, string? color, [FromServices] ApplicationDbContext context)
    {
        var query = context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .ThenInclude(v => v.Size)
            .AsQueryable();
        
        if (!string.IsNullOrEmpty(categoryName))
        {
            query = query.Where(p => p.Category!.Name == categoryName);
            ViewBag.CurrentCategory = categoryName;
        }
        
        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(p => p.Name.ToLower().Contains(searchString.ToLower()));
            ViewBag.Search = searchString;
        }
        
        if (sizeId.HasValue && sizeId.Value > 0)
        {
            query = query.Where(p => p.Variants.Any(v => v.SizeId == sizeId.Value));
            ViewBag.CurrentSizeId = sizeId;
        }
        
        if (!string.IsNullOrEmpty(color))
        {
            query = query.Where(p => p.Variants.Any(v => v.Color.ToLower() == color.ToLower()));
            ViewBag.CurrentColor = color;
        }

        var products = await query.ToListAsync();
        
        ViewBag.Categories = await _categoryRepository.GetAllAsync();
        ViewBag.Sizes = await context.Sizes.OrderBy(s => s.Id).ToListAsync();
        ViewBag.Colors = await context.ProductVariants
            .Where(v => !string.IsNullOrEmpty(v.Color))
            .Select(v => v.Color)
            .Distinct()
            .ToListAsync();
    
        return View(products);
    }
    
    public async Task<IActionResult> Details(int id, [FromServices] ApplicationDbContext context)
    {
        var product = await context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Tags)
            .Include(p => p.Reviews)
            .Include(p => p.Variants)
                .ThenInclude(v => v.Size)
            .Include(p => p.ProductQuestions)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return NotFound();

        ViewBag.EurRate = await _currencyService.GetExchangeRateAsync("EUR");
        ViewBag.UsdRate = await _currencyService.GetExchangeRateAsync("USD");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        bool isInWishlist = false;

        if (!string.IsNullOrEmpty(userId))
        {
            isInWishlist = await context.WishlistItems
                .AnyAsync(i => i.ProductId == id && i.Wishlist!.UserId == userId);
        }
        ViewBag.IsInWishlist = isInWishlist;

        return View(product);
    }
    
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReview(int productId, int rating, string comment, [FromServices] ApplicationDbContext context)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId) || rating < 1 || rating > 5 || string.IsNullOrWhiteSpace(comment))
        {
            return RedirectToAction(nameof(Details), new { id = productId });
        }

        var review = new Review
        {
            ProductId = productId,
            UserId = userId,
            Rating = rating,
            Comment = comment
        };

        context.Reviews.Add(review);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = productId });
    }
    
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReview(int reviewId, int productId, [FromServices] ApplicationDbContext context)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var review = await context.Reviews.FindAsync(reviewId);
        
        if (review == null || (review.UserId != userId && !User.IsInRole("Admin")))
        {
            return RedirectToAction(nameof(Details), new { id = productId });
        }

        context.Reviews.Remove(review);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = productId });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddQuestion(int productId, string question, [FromServices] ApplicationDbContext context)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(question))
            return RedirectToAction(nameof(Details), new { id = productId });

        var q = new ProductQuestion
        {
            ProductId = productId,
            UserId = userId!,
            Question = question,
            CreatedAt = DateTime.Now
        };

        context.ProductQuestions.Add(q);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = productId });
    }
}