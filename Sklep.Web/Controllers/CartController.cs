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

            List<CartItem> cartItems = new();

            if (cart != null)
            {
                cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cart);
            }

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            var cartJson = HttpContext.Session.GetString("Cart");

            List<CartItem> cartItems = new();

            if (!string.IsNullOrEmpty(cartJson))
            {
                cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
            }

            var existingItem =
                cartItems.FirstOrDefault(x => x.Product.Id == id);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cartItems.Add(new CartItem
                {
                    Product = product,
                    Quantity = 1
                });
            }

            HttpContext.Session.SetString(
                "Cart",
                JsonConvert.SerializeObject(cartItems));

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Increase(int id)
        {
            var cartJson = HttpContext.Session.GetString("Cart");

            if (cartJson == null)
                return RedirectToAction(nameof(Index));

            var cartItems =
                JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

            var item =
                cartItems.FirstOrDefault(x => x.Product.Id == id);

            if (item != null)
            {
                item.Quantity++;
            }

            HttpContext.Session.SetString(
                "Cart",
                JsonConvert.SerializeObject(cartItems));

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Decrease(int id)
        {
            var cartJson = HttpContext.Session.GetString("Cart");

            if (cartJson == null)
                return RedirectToAction(nameof(Index));

            var cartItems =
                JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

            var item =
                cartItems.FirstOrDefault(x => x.Product.Id == id);

            if (item != null)
            {
                item.Quantity--;

                if (item.Quantity <= 0)
                {
                    cartItems.Remove(item);
                }
            }

            HttpContext.Session.SetString(
                "Cart",
                JsonConvert.SerializeObject(cartItems));

            return RedirectToAction(nameof(Index));
        }
    }
}