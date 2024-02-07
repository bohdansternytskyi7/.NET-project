using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Models
{
    public class Product_Warehouse
    {
        public int IdProductWarehouse { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        [Required]
        public int IdWarehouse {  get; set; }
        [Required]
        public int IdProduct { get; set; }
        [Required]
        public int IdOrder { get; set; }
    }
}
