using System.ComponentModel.DataAnnotations;

namespace Sklep.Core.Models;

public class CurrencyExchangeRate : BaseEntity
{
    [Required]
    [StringLength(3)]
    public string CurrencyCode { get; set; } = string.Empty;

    [Required]
    public decimal Rate { get; set; }

    public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
}