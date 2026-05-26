using System.ComponentModel.DataAnnotations.Schema;

namespace Sklep.Core.Models;

public class WishlistItem : BaseEntity
{
    public int WishlistId { get; set; }
    
    [ForeignKey("WishlistId")]
    public Wishlist? Wishlist { get; set; }

    public int ProductId { get; set; }
    
    [ForeignKey("ProductId")]
    public Product? Product { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}