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
            .Include(p => p.Reviews)
            .ThenInclude(r => r.User)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    public void RemoveImage(ProductImage image)
    {
        _context.Set<ProductImage>().Remove(image);
    }
}