using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sklep.Infrastructure.Data;
using Sklep.Core.Models;
using System.Security.Claims;

namespace Sklep.Web.Controllers;

public class ProductQuestionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductQuestionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, string question)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var q = new ProductQuestion
        {
            ProductId = productId,
            UserId = userId!,
            Question = question,
            CreatedAt = DateTime.Now
        };

        _context.ProductQuestions.Add(q);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Products", new { id = productId });
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var questions = await _context.ProductQuestions
            .Include(x => x.Product)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return View(questions);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Answer(int id, string answer)
    {
        var q = await _context.ProductQuestions.FindAsync(id);

        if (q == null)
            return NotFound();

        q.Answer = answer;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}