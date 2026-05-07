using Microsoft.AspNetCore.Mvc;
using Sklep.Core.Interfaces;
using Sklep.Core.Models;

namespace Sklep.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    public async Task<IActionResult> Index()
    {
        var products = await _productRepository.GetAllWithCategoryAsync();
        return View(products);
    }
    
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productRepository.GetByIdWithCategoryAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }
}