using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models.Entities
{
    public class ShoppingCart
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string userId { get; set; } = Guid.NewGuid().ToString();
        public Product product { get; set; }
        public decimal? Price { get; set; }
        public int Quantity { get; set; }
    }
}
