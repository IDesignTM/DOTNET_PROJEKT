using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sklep.Infrastructure.Data;

namespace Sklep.Web.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductsApiController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.Description
            })
            .ToListAsync();

        return Ok(products);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _context.Products
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.Description
            })
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound(new { Message = $"Produkt o ID {id} nie istnieje." });

        return Ok(product);
    }
}