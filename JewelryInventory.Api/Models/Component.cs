namespace JewelryInventory.Api.Models;

public class Component
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string? Category { get; set; }
    public string UnitOfMeasurement { get; set; } = "pcs";
    public decimal UnitCost { get; set; }
    public decimal LowStockThreshold { get; set; }
    public string? StorageLocation  { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Barcode { get; set; }
    
    public ICollection<ComponentInventoryMovement> InventoryMovements { get; set; }
        = new List<ComponentInventoryMovement>();
}