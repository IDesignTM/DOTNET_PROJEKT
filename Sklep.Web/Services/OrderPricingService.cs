using Sklep.Core.Models;

namespace Sklep.Web.Services;

public class OrderPricingService
{
    public decimal CalculateProductsTotal(List<CartItem> cart)
    {
        return cart.Sum(x => x.Product.Price * x.Quantity);
    }

    public decimal ApplyDiscount(decimal total, decimal percent)
    {
        var result = total - (total * percent / 100);

        return result;
    }

    public decimal AddShipping(decimal total, decimal shippingCost)
    {
        return total + shippingCost;
    }
}