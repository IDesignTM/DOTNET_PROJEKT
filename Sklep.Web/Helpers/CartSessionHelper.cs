using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Sklep.Core.Models;

namespace Sklep.Web.Helpers
{
    public static class CartSessionHelper
    {
        public static int GetCartCount(ISession session)
        {
            var cart = session.GetString("Cart");

            if (string.IsNullOrEmpty(cart))
                return 0;

            var products = JsonConvert.DeserializeObject<List<Product>>(cart);

            return products?.Count ?? 0;
        }
    }
}