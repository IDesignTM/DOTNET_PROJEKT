using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    public async Task<IActionResult> Index()
    {
        var products = await _productRepository.GetAllWithCategoryAsync();
        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productRepository.GetByIdWithCategoryAsync(id);
        if (product == null)
            return NotFound();

        return View(product);
    }

    public async Task<IActionResult> Create()
    {
        await LoadCategoriesAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            return View(product);
        }

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        await LoadCategoriesAsync();
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Product product)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            return View(product);
        }

        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productRepository.GetByIdWithCategoryAsync(id);
        if (product == null)
            return NotFound();

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        _productRepository.Delete(product);
        await _productRepository.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadCategoriesAsync()
    {
        var categories = (await _categoryRepository.GetAllAsync())
            .OrderBy(c => c.Name)
            .ToList();

        ViewBag.Categories = new SelectList(categories, "Id", "Name");
    }
}