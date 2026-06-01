using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sklep.Core.DTOs;
using Sklep.Core.Models;
using Sklep.Infrastructure.Data;

namespace Sklep.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class TagsController : Controller
{
    private readonly ApplicationDbContext _context;

    public TagsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        var tags = await _context.Tags.OrderBy(t => t.Name).ToListAsync();
        return View(tags);
    }
    
    public IActionResult Create()
    {
        return View(new TagDto());
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TagDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var tag = new Tag
        {
            Name = dto.Name
        };

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Edit(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null) return NotFound();

        var dto = new TagDto
        {
            Id = tag.Id,
            Name = tag.Name
        };

        return View(dto);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TagDto dto)
    {
        if (id != dto.Id) return NotFound();

        if (!ModelState.IsValid) return View(dto);

        var tag = await _context.Tags.FindAsync(id);
        if (tag == null) return NotFound();

        tag.Name = dto.Name;

        _context.Tags.Update(tag);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null) return NotFound();
        
        var dto = new TagDto
        {
            Id = tag.Id,
            Name = tag.Name
        };

        return View(dto);
    }
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null) return NotFound();

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}