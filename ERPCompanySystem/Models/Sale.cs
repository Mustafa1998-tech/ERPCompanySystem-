namespace ERPCompanySystem.Models;

public class Sale
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // Cash, Credit, Bank Transfer
    public string Status { get; set; } = "Completed"; // Completed, Pending, Cancelled
    public string Notes { get; set; } = string.Empty;
}
