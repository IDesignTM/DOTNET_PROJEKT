using Microsoft.AspNetCore.Mvc;
using Sklep.Core.Interfaces;
using Sklep.Core.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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
    
    public async Task<IActionResult> Index(string? categoryName, string? searchString, int? sizeId, string? color)
    {
        var products = await _productRepository.GetFilteredProductsAsync(categoryName, searchString, sizeId, color);
        
        ViewBag.CurrentCategory = categoryName;
        ViewBag.Search = searchString;
        ViewBag.CurrentSizeId = sizeId;
        ViewBag.CurrentColor = color;

        ViewBag.Categories = await _categoryRepository.GetAllAsync();
        ViewBag.Sizes = await _productRepository.GetAvailableSizesAsync();
        ViewBag.Colors = await _productRepository.GetAvailableColorsAsync();
    
        return View(products);
    }
    
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productRepository.GetByIdWithCategoryAsync(id);

        if (product == null) return NotFound();

        ViewBag.EurRate = await _currencyService.GetExchangeRateAsync("EUR");
        ViewBag.UsdRate = await _currencyService.GetExchangeRateAsync("USD");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        bool isInWishlist = false;

        if (!string.IsNullOrEmpty(userId))
        {
            isInWishlist = await _productRepository.IsProductInWishlistAsync(id, userId);

            await _productRepository.AddProductViewHistoryAsync(new ProductViewHistory
            {
                UserId = userId,
                ProductId = id,
                ViewedAt = DateTime.Now
            });

            await _productRepository.SaveChangesAsync();
        }
        ViewBag.IsInWishlist = isInWishlist;

        return View(product);
    }
    
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReview(int productId, int rating, string comment)
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

        await _productRepository.AddReviewAsync(review);
        await _productRepository.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = productId });
    }
    
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReview(int reviewId, int productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var review = await _productRepository.GetReviewByIdAsync(reviewId);
        
        if (review == null || (review.UserId != userId && !User.IsInRole("Admin")))
        {
            return RedirectToAction(nameof(Details), new { id = productId });
        }

        _productRepository.DeleteReview(review);
        await _productRepository.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = productId });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddQuestion(int productId, string question)
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

        await _productRepository.AddQuestionAsync(q);
        await _productRepository.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = productId });
    }
}