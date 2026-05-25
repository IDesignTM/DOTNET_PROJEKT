using System.ComponentModel.DataAnnotations;

namespace Sklep.Core.DTOs;

public abstract class ProductBaseDto
{
    [Required(ErrorMessage = "Nazwa jest wymagana.")]
    [StringLength(150, ErrorMessage = "Nazwa nie może przekraczać 150 znaków.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Opis jest wymagany.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Cena jest wymagana.")]
    [Range(0.01, 100000, ErrorMessage = "Podaj prawidłową cenę.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Wybierz kategorię.")]
    [Range(1, int.MaxValue, ErrorMessage = "Wybierz kategorię z listy.")]
    public int CategoryId { get; set; }
}