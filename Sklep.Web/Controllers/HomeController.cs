using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sklep.Core.Interfaces;
using Sklep.Web.Models;

namespace Sklep.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductRepository _productRepository;

    public HomeController(ILogger<HomeController> logger, IProductRepository productRepository)
    {
        _logger = logger;
        _productRepository = productRepository;
    }

    public async Task<IActionResult> Index()
    {
        // U¿ywamy metody, która na 100% dzia³a i pobiera Twoje produkty
        var products = await _productRepository.GetAllWithCategoryAsync();

        return View(products);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}