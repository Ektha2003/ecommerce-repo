using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class CatagoryViewModel
    {
        [StringLength(50)]
        [Required(ErrorMessage = "Catagory name Required")]
        public string? CatagoryName { get; set; }
        public string? CatagoryId { get; set; } = Guid.NewGuid().ToString();
    }
}
