using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Models
{
    public class Product
    {
        public int IdProduct { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
