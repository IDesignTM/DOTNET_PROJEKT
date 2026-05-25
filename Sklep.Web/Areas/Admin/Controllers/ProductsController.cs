using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sklep.Core.Interfaces;
using Sklep.Core.Models;
using Sklep.Core.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Sklep.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly IBaseRepository<Category> _categoryRepository;
    private readonly IBaseRepository<Tag> _tagRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductsController(
        IProductRepository productRepository,
        IBaseRepository<Category> categoryRepository,
        IBaseRepository<Tag> tagRepository,
        IWebHostEnvironment webHostEnvironment)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _tagRepository = tagRepository;
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
    
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        await LoadCategoriesAsync();
        await LoadTagsAsync();
        
        return View(new ProductCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            await LoadTagsAsync();
            return View(dto);
        }
        
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            CategoryId = dto.CategoryId
        };

        if (dto.SelectedTags != null && dto.SelectedTags.Any())
        {
            var allTags = await _tagRepository.GetAllAsync();
            product.Tags = allTags.Where(t => dto.SelectedTags.Contains(t.Id)).ToList();
        }
        
        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();
        
        if (dto.Images != null && dto.Images.Any())
        {
            await SaveProductImages(product, dto.Images);
            await _productRepository.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
    
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productRepository.GetByIdWithCategoryAsync(id);
        if (product == null) return NotFound();

        var dto = new ProductEditDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            SelectedTags = product.Tags?.Select(t => t.Id).ToList() ?? new List<int>(),
            ExistingImages = product.Images?.Select(i => new ProductImageDto 
            { 
                Id = i.Id, 
                ImagePath = i.ImagePath 
            }).ToList() ?? new List<ProductImageDto>()
        };

        await LoadCategoriesAsync();
        await LoadTagsAsync();
        
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, ProductEditDto dto)
    {
        if (id != dto.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            await LoadTagsAsync();
            
            var tempProduct = await _productRepository.GetByIdWithCategoryAsync(id);
            if (tempProduct != null)
            {
                dto.ExistingImages = tempProduct.Images?.Select(i => new ProductImageDto 
                { 
                    Id = i.Id, 
                    ImagePath = i.ImagePath 
                }).ToList() ?? new List<ProductImageDto>();
            }
            
            return View(dto);
        }
        
        var existingProduct = await _productRepository.GetByIdWithCategoryAsync(id);
        if (existingProduct == null) return NotFound();
        
        existingProduct.Name = dto.Name;
        existingProduct.Description = dto.Description;
        existingProduct.Price = dto.Price;
        existingProduct.CategoryId = dto.CategoryId;
        
        existingProduct.Tags.Clear();
        if (dto.SelectedTags != null && dto.SelectedTags.Any())
        {
            var allTags = await _tagRepository.GetAllAsync();
            var tagsToAdd = allTags.Where(t => dto.SelectedTags.Contains(t.Id)).ToList();
            foreach (var tag in tagsToAdd)
            {
                existingProduct.Tags.Add(tag);
            }
        }
        
        if (dto.ImagesToDelete != null && dto.ImagesToDelete.Any())
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            foreach (var imgId in dto.ImagesToDelete)
            {
                var imgToRemove = existingProduct.Images.FirstOrDefault(i => i.Id == imgId);
                if (imgToRemove != null)
                {
                    var filePath = Path.Combine(wwwRootPath, imgToRemove.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                    _productRepository.RemoveImage(imgToRemove);
                }
            }
        }
        
        if (dto.Images != null && dto.Images.Any()) 
        {
            await SaveProductImages(existingProduct, dto.Images);
        }
        
        _productRepository.Update(existingProduct);
        await _productRepository.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productRepository.GetByIdWithCategoryAsync(id);
        if (product == null)
            return NotFound();

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
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
    
    private async Task LoadTagsAsync()
    {
        var tags = (await _tagRepository.GetAllAsync()).OrderBy(t => t.Name).ToList();
        ViewBag.Tags = tags;
    }
}