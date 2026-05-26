using System.ComponentModel.DataAnnotations;

namespace Sklep.Core.Models;

public class Wishlist : BaseEntity
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
}