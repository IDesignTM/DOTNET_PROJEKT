using Microsoft.AspNetCore.Mvc;
using Sklep.Core.Interfaces;
using Sklep.Core.Models;
using Microsoft.AspNetCore.Authorization;

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

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (!ModelState.IsValid)
            return View(category);

        await _repo.AddAsync(category);
        await _repo.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category == null) return NotFound();

        return View(category);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Category category)
    {
        if (!ModelState.IsValid)
            return View(category);

        _repo.Update(category);
        await _repo.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category == null) return NotFound();

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
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