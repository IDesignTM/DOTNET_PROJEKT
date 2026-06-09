using Sklep.Core.Models;

namespace Sklep.Core.Interfaces;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<IEnumerable<Product>> GetAllWithCategoryAsync();
    Task<Product?> GetByIdWithCategoryAsync(int id);
    void RemoveImage(ProductImage image);
    
    Task<IEnumerable<Product>> GetFilteredProductsAsync(string? categoryName, string? searchString, int? sizeId, string? color);
    Task<IEnumerable<Size>> GetAvailableSizesAsync();
    Task<IEnumerable<string>> GetAvailableColorsAsync();
    Task<bool> IsProductInWishlistAsync(int productId, string userId);
    Task AddProductViewHistoryAsync(ProductViewHistory history);
    Task AddReviewAsync(Review review);
    Task<Review?> GetReviewByIdAsync(int reviewId);
    void DeleteReview(Review review);
    Task AddQuestionAsync(ProductQuestion question);
}