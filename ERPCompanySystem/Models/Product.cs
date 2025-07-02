namespace ERPCompanySystem.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public int StockQuantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
