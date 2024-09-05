using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models.Entities
{
    public class OrderItem
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public int Amount { get; set; }
        public double Price { get; set; }

        public string ProductId { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("MovieId")]
        public Product product { get; set; }
        [ForeignKey("OrderId")]
        public int OrderId { get; set; } 
        public Order Order { get; set; }
    }
}
