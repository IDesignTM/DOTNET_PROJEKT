namespace Sklep.Core.Models
{
    public class DiscountCode
    {
        public int Id { get; set; }

        public string Code { get; set; } = "";

        public decimal DiscountPercent { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime ExpirationDate { get; set; }
        public ICollection<DiscountCodeUsage> Usages { get; set; } = new List<DiscountCodeUsage>();
    }
}