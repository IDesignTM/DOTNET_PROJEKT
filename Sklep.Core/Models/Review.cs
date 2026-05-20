using System.ComponentModel.DataAnnotations;

namespace Sklep.Core.Models;

public class Review : BaseEntity
{
    [Required]
    [Range(1, 5, ErrorMessage = "Ocena musi być między 1 a 5.")]
    public int Rating { get; set; }

    [Required(ErrorMessage = "Treść opinii jest wymagana.")]
    [StringLength(2000)]
    public string Comment { get; set; } = string.Empty;

    // Relacja do Produktu
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

    // Relacja do Użytkownika (Identity używa string jako ID)
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;
}