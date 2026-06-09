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
            
            var items = JsonConvert.DeserializeObject<List<CartItem>>(cart);
            
            return items?.Sum(i => i.Quantity) ?? 0; 
        }
    }
}