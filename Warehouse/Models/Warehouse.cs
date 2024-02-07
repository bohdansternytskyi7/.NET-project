using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Models
{
    public class Warehouse
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        [Required]
        [MaxLength(200)]
        public string Address { get; set; }
    }
}
