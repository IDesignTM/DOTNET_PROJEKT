using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sklep.Core.Interfaces;
using Sklep.Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace Sklep.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly IBaseRepository<Category> _categoryRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductsController(
        IProductRepository productRepository,
        IBaseRepository<Category> categoryRepository,
        IWebHostEnvironment webHostEnvironment)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _webHostEnvironment = webHostEnvironment;
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
    public async Task<IActionResult> Create(Product product, List<IFormFile> images)
    {
        ModelState.Remove("Images");
        ModelState.Remove("Category");

        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            return View(product);
        }
        
        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();
        
        if (images != null && images.Any())
        {
            await SaveProductImages(product, images);
            await _productRepository.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productRepository.GetByIdWithCategoryAsync(id);
        if (product == null)
            return NotFound();

        await LoadCategoriesAsync();
        return View(product);
    }

    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, Product product, List<IFormFile> images, List<int> imagesToDelete)
{
    if (id != product.Id) return NotFound();

    ModelState.Remove("Images");
    ModelState.Remove("Category");

    if (!ModelState.IsValid)
    {
        await LoadCategoriesAsync();
        return View(product);
    }
    
    var existingProduct = await _productRepository.GetByIdWithCategoryAsync(id);
    if (existingProduct == null) return NotFound();
    
    existingProduct.Name = product.Name;
    existingProduct.Description = product.Description;
    existingProduct.Price = product.Price;
    existingProduct.CategoryId = product.CategoryId;
    
    if (imagesToDelete != null && imagesToDelete.Any())
    {
        string wwwRootPath = _webHostEnvironment.WebRootPath;

        foreach (var imgId in imagesToDelete)
        {
            var imgToRemove = existingProduct.Images.FirstOrDefault(i => i.Id == imgId);
            if (imgToRemove != null)
            {
                var filePath = Path.Combine(wwwRootPath, imgToRemove.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                
                _productRepository.RemoveImage(imgToRemove);
            }
        }
    }
    
    if (images != null && images.Any())
    {
        await SaveProductImages(existingProduct, images);
    }
    
    _productRepository.Update(existingProduct);
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
        var product = await _productRepository.GetByIdWithCategoryAsync(id);
    
        if (product == null) return NotFound();
        
        if (product.Images != null && product.Images.Any())
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            foreach (var img in product.Images)
            {
                var filePath = Path.Combine(wwwRootPath, img.ImagePath.TrimStart('/'));

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }
        
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
    
    private async Task SaveProductImages(Product product, List<IFormFile> images)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        string wwwRootPath = _webHostEnvironment.WebRootPath;
        string uploadDir = Path.Combine(wwwRootPath, "uploads", "products");

        if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

        foreach (var file in images)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            
            if (!allowedExtensions.Contains(extension)) continue; 

            string fileName = Guid.NewGuid().ToString() + extension;
            string filePath = Path.Combine(uploadDir, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            product.Images.Add(new ProductImage
            {
                ImagePath = "/uploads/products/" + fileName,
                ProductId = product.Id
            });
        }
    }
}