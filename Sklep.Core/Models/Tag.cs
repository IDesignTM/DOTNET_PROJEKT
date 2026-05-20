using System.ComponentModel.DataAnnotations;

namespace Sklep.Core.Models;

public class Tag : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
    
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}