namespace Sklep.Core.Models;

public class ProductViewHistory
{
    public int Id { get; set; }

    public string UserId { get; set; } = "";

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public DateTime ViewedAt { get; set; } = DateTime.Now;
}