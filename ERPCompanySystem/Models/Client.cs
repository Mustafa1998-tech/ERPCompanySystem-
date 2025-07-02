namespace ERPCompanySystem.Models;

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
