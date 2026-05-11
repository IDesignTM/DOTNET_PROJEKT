namespace Sklep.Core.Models;

public class ProductImage : BaseEntity
{
    public string ImagePath { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
}