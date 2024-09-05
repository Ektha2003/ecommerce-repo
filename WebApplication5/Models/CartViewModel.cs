using WebApplication5.Models.Entities;

namespace WebApplication5.Models
{
    public class CartViewModel
    {
        public List<ShoppingCart> Items { get; set; } = new List<ShoppingCart>();
        public decimal? SubTotal
        {
            get
            {
                return Items.Sum(item => item.product.Price * item.Quantity);
            }
        }
    }
    
}
