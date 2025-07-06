using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPCompanySystem.Models
{
    [Table("Employees")]
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = null!;

        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [Required]
        public DateTime HireDate { get; set; }

        public bool IsActive { get; set; } = true;

        public int? DepartmentId { get; set; }

        // Navigation properties
        public Department? Department { get; set; }
    }
}
