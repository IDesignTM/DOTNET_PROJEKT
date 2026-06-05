namespace Sklep.Core.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public decimal Amount { get; set; }

        public string Method { get; set; } = "Płatność online";

        public string Status { get; set; } = "Oczekująca";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? PaidAt { get; set; }
    }
}