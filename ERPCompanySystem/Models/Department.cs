using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPCompanySystem.Models
{
    [Table("Departments")]
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(250)]
        public string? Description { get; set; }

        // Navigation property
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
