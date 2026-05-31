using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sklep.Core.Models;

public class ProductVariant : BaseEntity
{
    public int ProductId { get; set; }
    
    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }

    public int SizeId { get; set; }
    
    [ForeignKey("SizeId")]
    public virtual Size? Size { get; set; }

    [Required]
    [Range(0, 10000, ErrorMessage = "Stan magazynowy nie może być ujemny.")]
    public int StockQuantity { get; set; }

    [StringLength(50)]
    public string Color { get; set; } = string.Empty;
}