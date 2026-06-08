namespace Sklep.Core.Models;

public class CartItem
{
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
}