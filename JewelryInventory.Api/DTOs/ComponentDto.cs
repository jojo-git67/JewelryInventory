namespace JewelryInventory.Api.DTOs;

public class ComponentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string? Category { get; set; }
    public string UnitOfMeasurement { get; set; } = string.Empty;
    public decimal UnitCost { get; set; }
    public decimal LowStockThreshold { get; set; }
    public string? StorageLocation { get; set; }
    public bool IsActive { get; set; }
    public string? Barcode { get; set; }
    
    public decimal CurrentStock { get; set; }
}