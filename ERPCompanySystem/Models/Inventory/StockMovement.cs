namespace ERPCompanySystem.Models.Inventory;

public class StockMovement
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public DateTime MovementDate { get; set; }
    public string MovementType { get; set; } = string.Empty; // IN, OUT, ADJUSTMENT
    public int Quantity { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty; // Invoice number or document reference
    public string Description { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
}
