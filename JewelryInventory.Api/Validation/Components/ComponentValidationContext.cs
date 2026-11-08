namespace JewelryInventory.Api.Validation.Components;

public class ComponentValidationContext
{
    public int? ComponentId { get; set; }
    public string? Name { get; set; }
    public string? UnitOfMeasurement { get; set; }
    public string? Barcode { get; set; }
    public decimal UnitCost { get; set; }
    public decimal LowStockThreshold { get; set; }
    public decimal? InitialStock { get; set; }
}