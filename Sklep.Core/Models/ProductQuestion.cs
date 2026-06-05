namespace Sklep.Core.Models
{
    public class ProductQuestion
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string UserId { get; set; } = "";

        public string Question { get; set; } = "";

        public string? Answer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}