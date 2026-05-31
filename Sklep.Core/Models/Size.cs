using System.ComponentModel.DataAnnotations;

namespace Sklep.Core.Models;

public class Size : BaseEntity
{
    [Required]
    [StringLength(10)]
    public string Name { get; set; } = string.Empty;
}