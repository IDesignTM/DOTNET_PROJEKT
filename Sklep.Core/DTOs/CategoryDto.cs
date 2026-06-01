using System.ComponentModel.DataAnnotations;

namespace Sklep.Core.DTOs;

public class CategoryDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nazwa kategorii jest wymagana.")]
    [StringLength(100, ErrorMessage = "Nazwa kategorii nie może być dłuższa niż 100 znaków.")]
    public string Name { get; set; } = string.Empty;
}