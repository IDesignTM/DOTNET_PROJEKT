using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sklep.Core.Interfaces;
using Sklep.Core.Models;

namespace Sklep.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;

        public CartController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetString("Cart");

            List<Product> products = new();

            if (cart != null)
            {
                products = JsonConvert.DeserializeObject<List<Product>>(cart);
            }

            return View(products);
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            var cart = HttpContext.Session.GetString("Cart");

            List<Product> products = new();

            if (cart != null)
            {
                products = JsonConvert.DeserializeObject<List<Product>>(cart);
            }

            products.Add(product);

            HttpContext.Session.SetString(
                "Cart",
                JsonConvert.SerializeObject(products));

            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetString("Cart");

            if (cart != null)
            {
                var products = JsonConvert.DeserializeObject<List<Product>>(cart);

                var product = products.FirstOrDefault(p => p.Id == id);

                if (product != null)
                {
                    products.Remove(product);
                }

                HttpContext.Session.SetString(
                    "Cart",
                    JsonConvert.SerializeObject(products));
            }

            return RedirectToAction("Index");
        }
    }
}