using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Models
{
    public class Order
    {
        public int IdOrder { get; set; }
        [Required]
        public int IdProduct { get; set; }
        public int Amount { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime FulfilledAt { get; set; }
    }
}
