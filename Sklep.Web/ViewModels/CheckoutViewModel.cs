using System.ComponentModel.DataAnnotations;

namespace Sklep.Web.ViewModels;

public class CheckoutViewModel
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    public int? SelectedAddressId { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? PostalCode { get; set; }

    public string? DiscountCode { get; set; }
}