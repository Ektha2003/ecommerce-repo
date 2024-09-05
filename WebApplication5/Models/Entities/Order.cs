using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models.Entities
{
    public class Order
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString(); 

        public string Email { get; set; }

        public string UserId { get; set; } =Guid.NewGuid().ToString();
       
        public List<OrderItem> OrderItems { get; set; }
    }
}
