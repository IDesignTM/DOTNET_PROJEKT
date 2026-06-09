using Sklep.Core.Models;
using Sklep.Web.Services;
using Xunit;

public class OrderPricingServiceTests
{
    private readonly OrderPricingService _service = new();

    [Fact]
    public void Should_Calculate_Cart_Total()
    {
        var cart = new List<CartItem>
        {
            new CartItem
            {
                Product = new Product { Price = 100 },
                Quantity = 2
            }
        };

        var result = _service.CalculateProductsTotal(cart);

        Assert.Equal(200, result);
    }

    [Fact]
    public void Should_Apply_Discount()
    {
        var result = _service.ApplyDiscount(200, 10);

        Assert.Equal(180, result);
    }

    [Fact]
    public void Should_Add_Shipping()
    {
        var result = _service.AddShipping(100, 15);

        Assert.Equal(115, result);
    }

    [Fact]
    public void Should_Calculate_Full_Order_Price()
    {
        var cart = new List<CartItem>
        {
            new CartItem { Product = new Product { Price = 100 }, Quantity = 2 }
        };

        var service = new OrderPricingService();

        var productsTotal = service.CalculateProductsTotal(cart);
        var afterDiscount = service.ApplyDiscount(productsTotal, 10);
        var final = service.AddShipping(afterDiscount, 15);

        Assert.Equal(195, final);
    }

    [Fact]
    public void Should_Not_Change_Total_When_No_Discount()
    {
        var total = 200;

        var result = new OrderPricingService()
            .ApplyDiscount(total, 0);

        Assert.Equal(200, result);
    }

    [Fact]
    public void Should_Not_Change_Total_When_Shipping_Is_Free()
    {
        var total = 200;

        var result = new OrderPricingService()
            .AddShipping(total, 0);

        Assert.Equal(200, result);
    }

    [Fact]
    public void Should_Handle_100Percent_Discount()
    {
        var result = new OrderPricingService()
            .ApplyDiscount(200, 100);

        Assert.Equal(0, result);
    }

    [Fact]
    public void Should_Return_Zero_For_Empty_Cart()
    {
        var service = new OrderPricingService();

        var result = service.CalculateProductsTotal(new List<CartItem>());

        Assert.Equal(0, result);
    }
}