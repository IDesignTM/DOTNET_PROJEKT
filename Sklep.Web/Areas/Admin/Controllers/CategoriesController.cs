using Microsoft.AspNetCore.Mvc;
using Sklep.Core.Interfaces;
using Sklep.Core.Models;
using Sklep.Core.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Sklep.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly IBaseRepository<Category> _repo;

    public CategoriesController(IBaseRepository<Category> repo)
    {
        _repo = repo;
    }
    
    public async Task<IActionResult> Index()
    {
        var categories = await _repo.GetAllAsync();
        return View(categories);
    }
    
    public IActionResult Create()
    {
        return View(new CategoryDto());
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);
        
        var category = new Category
        {
            Name = dto.Name
        };

        await _repo.AddAsync(category);
        await _repo.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category == null) return NotFound();
        
        var dto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name
        };

        return View(dto);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryDto dto)
    {
        if (id != dto.Id) return NotFound();

        if (!ModelState.IsValid)
            return View(dto);
        
        var category = await _repo.GetByIdAsync(id);
        if (category == null) return NotFound();
        
        category.Name = dto.Name;

        _repo.Update(category);
        await _repo.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category == null) return NotFound();
        
        var dto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name
        };

        return View(dto);
    }
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category == null) return NotFound();

        _repo.Delete(category);
        await _repo.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}