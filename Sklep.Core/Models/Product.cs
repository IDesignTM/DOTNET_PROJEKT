using System.ComponentModel.DataAnnotations;

namespace Sklep.Core.Models;

public class Product : BaseEntity
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 9999999, ErrorMessage = "Cena musi być dodatnia.")]
    public decimal Price { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Wybierz kategorię")]
    public int CategoryId { get; set; }

    public virtual Category? Category { get; set; }
    
    public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public virtual ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public ICollection<ProductQuestion> ProductQuestions { get; set; } = new List<ProductQuestion>();
}