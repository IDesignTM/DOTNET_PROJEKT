using Microsoft.AspNetCore.Mvc;
using Sklep.Core.Interfaces;
using Sklep.Core.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Sklep.Infrastructure.Data;

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
    
    public async Task<IActionResult> Index(string? categoryName, string? searchString)
    {
        var products = await _productRepository.GetAllWithCategoryAsync();
    
        if (!string.IsNullOrEmpty(categoryName))
        {
            products = products.Where(p => p.Category?.Name == categoryName).ToList();
            ViewBag.CurrentCategory = categoryName;
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            products = products
                .Where(p => p.Name.ToLower().Contains(searchString.ToLower()))
                .ToList();

            ViewBag.Search = searchString;
        }

        ViewBag.Categories = await _categoryRepository.GetAllAsync();
    
        return View(products);
    }
    
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productRepository.GetByIdWithCategoryAsync(id);
        if (product == null) return NotFound();
        
        ViewBag.EurRate = await _currencyService.GetExchangeRateAsync("EUR");
        ViewBag.UsdRate = await _currencyService.GetExchangeRateAsync("USD");

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
}