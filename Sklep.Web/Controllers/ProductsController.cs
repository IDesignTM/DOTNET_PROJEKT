using Microsoft.AspNetCore.Mvc;
using Sklep.Core.Interfaces;
using Sklep.Core.Models;

namespace Sklep.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly IBaseRepository<Category> _categoryRepository;

    public ProductsController(
        IProductRepository productRepository, 
        IBaseRepository<Category> categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }
    
    public async Task<IActionResult> Index(string? categoryName)
    {
        var products = await _productRepository.GetAllWithCategoryAsync();
    
        if (!string.IsNullOrEmpty(categoryName))
        {
            products = products.Where(p => p.Category?.Name == categoryName).ToList();
            ViewBag.CurrentCategory = categoryName;
        }
        
        ViewBag.Categories = await _categoryRepository.GetAllAsync();
    
        return View(products);
    }
    
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productRepository.GetByIdWithCategoryAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }
}