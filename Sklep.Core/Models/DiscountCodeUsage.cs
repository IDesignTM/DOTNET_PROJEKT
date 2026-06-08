namespace Sklep.Core.Models;

public class DiscountCodeUsage
{
    public int Id { get; set; }

    public int DiscountCodeId { get; set; }
    public DiscountCode DiscountCode { get; set; } = null!;

    public string UserId { get; set; } = "";

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public DateTime UsedAt { get; set; } = DateTime.Now;
}