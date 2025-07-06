using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPCompanySystem.Models
{
    [Table("Warehouses")]
    public class Warehouse
    {
        [Key]
        public int WarehouseId { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        [StringLength(250)]
        public string? Location { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
