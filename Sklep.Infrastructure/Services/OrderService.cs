using Sklep.Core.Interfaces;
using Sklep.Core.Models;
using Sklep.Infrastructure.Data;

namespace Sklep.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateOrderAsync(string userId, int productId)
    {
        var product = await _context.Products.FindAsync(productId);

        if (product == null)
            throw new Exception("Product not found");

        var order = new Order
        {
            UserId = userId,
            TotalPrice = product.Price
        };

        order.Items.Add(new OrderItem
        {
            ProductId = product.Id,
            Quantity = 1,
            UnitPrice = product.Price
        });

        _context.Orders.Add(order);

        await _context.SaveChangesAsync();

        return order.Id;
    }
}