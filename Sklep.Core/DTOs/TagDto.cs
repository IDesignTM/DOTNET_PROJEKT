using System.ComponentModel.DataAnnotations;

namespace Sklep.Core.DTOs;

public class TagDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nazwa tagu jest wymagana.")]
    [StringLength(50, ErrorMessage = "Nazwa tagu nie może być dłuższa niż 50 znaków.")]
    public string Name { get; set; } = string.Empty;
}