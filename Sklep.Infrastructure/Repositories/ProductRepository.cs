using Microsoft.EntityFrameworkCore;
using Sklep.Core.Interfaces;
using Sklep.Core.Models;
using Sklep.Infrastructure.Data;

namespace Sklep.Infrastructure.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetAllWithCategoryAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Tags)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdWithCategoryAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Tags)
            .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
            .Include(p => p.Variants)
                .ThenInclude(v => v.Size)
            .Include(p => p.ProductQuestions)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    public void RemoveImage(ProductImage image)
    {
        _context.Set<ProductImage>().Remove(image);
    }

    public async Task<IEnumerable<Product>> GetFilteredProductsAsync(string? categoryName, string? searchString, int? sizeId, string? color)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .ThenInclude(v => v.Size)
            .AsQueryable();
        
        if (!string.IsNullOrEmpty(categoryName))
            query = query.Where(p => p.Category!.Name == categoryName);
        
        if (!string.IsNullOrEmpty(searchString))
            query = query.Where(p => p.Name.ToLower().Contains(searchString.ToLower()));
        
        if (sizeId.HasValue && sizeId.Value > 0)
            query = query.Where(p => p.Variants.Any(v => v.SizeId == sizeId.Value));
        
        if (!string.IsNullOrEmpty(color))
            query = query.Where(p => p.Variants.Any(v => v.Color.ToLower() == color.ToLower()));

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Size>> GetAvailableSizesAsync()
    {
        return await _context.Sizes.OrderBy(s => s.Id).ToListAsync();
    }

    public async Task<IEnumerable<string>> GetAvailableColorsAsync()
    {
        return await _context.ProductVariants
            .Where(v => !string.IsNullOrEmpty(v.Color))
            .Select(v => v.Color)
            .Distinct()
            .ToListAsync();
    }

    public async Task<bool> IsProductInWishlistAsync(int productId, string userId)
    {
        return await _context.WishlistItems
            .AnyAsync(i => i.ProductId == productId && i.Wishlist!.UserId == userId);
    }

    public async Task AddProductViewHistoryAsync(ProductViewHistory history)
    {
        await _context.ProductViewHistories.AddAsync(history);
    }

    public async Task AddReviewAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
    }

    public async Task<Review?> GetReviewByIdAsync(int reviewId)
    {
        return await _context.Reviews.FindAsync(reviewId);
    }

    public void DeleteReview(Review review)
    {
        _context.Reviews.Remove(review);
    }

    public async Task AddQuestionAsync(ProductQuestion question)
    {
        await _context.ProductQuestions.AddAsync(question);
    }
}