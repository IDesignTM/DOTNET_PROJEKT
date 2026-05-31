using System.ComponentModel.DataAnnotations;

namespace Sklep.Core.Models.DTOs;

public class ProductVariantDto
{
    public int Id { get; set; }
    
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Wybierz rozmiar.")]
    public int SizeId { get; set; }

    [Required(ErrorMessage = "Podaj stan magazynowy.")]
    [Range(0, 10000, ErrorMessage = "Stan magazynowy nie może być ujemny.")]
    public int StockQuantity { get; set; }

    [StringLength(50, ErrorMessage = "Nazwa koloru jest za długa.")]
    public string Color { get; set; } = string.Empty;
}