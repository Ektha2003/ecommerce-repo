using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models.Entities
{
    public class Catagory
    {
        [Key]
        public string CatagoryId { get; set; }= Guid.NewGuid().ToString();

        [StringLength(50)]
        [Required(ErrorMessage ="Catagory name Required")]
        public string? CatagoryName { get; set; }


    }
}
