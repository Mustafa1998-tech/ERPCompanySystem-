namespace ERPCompanySystem.Models;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
