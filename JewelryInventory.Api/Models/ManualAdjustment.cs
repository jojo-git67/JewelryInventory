namespace JewelryInventory.Api.Models;

public class ManualAdjustment
{
    public int Id { get; set; }

    public int ComponentId { get; set; }
    public Component Component { get; set; } = null!;
    
    public decimal QuantityChange { get; set; }
    public string Reason {get; set;} = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ComponentInventoryMovement?  InventoryMovement { get; set; }

}