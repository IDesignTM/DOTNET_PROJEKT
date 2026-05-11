using Sklep.Core.Models;

namespace Sklep.Core.Interfaces;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<IEnumerable<Product>> GetAllWithCategoryAsync();
    Task<Product?> GetByIdWithCategoryAsync(int id);
    void RemoveImage(ProductImage image);
}