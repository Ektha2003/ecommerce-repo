using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models.Entities
{
    public class Product
    {
        [Key]
        public string ProductId { get; set; } = Guid.NewGuid().ToString();

        [StringLength(40)]
        [Required]
        public string? ProductName { get; set; }

        [StringLength(100)]
        [Required]
        public string? ProductDescription { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public string CategoryId { get; set; }
       
        public string? ProductImg
        {
            get; set;

        }
        [Required]
        [Range(typeof(int),"1","500", ErrorMessage ="invalid quantity" )]
        public int? Quantity { get; set; }

        [Required]
        public decimal? Price { get; set; }
    }
}
