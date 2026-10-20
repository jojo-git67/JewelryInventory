namespace JewelryInventory.Api.Models;

public class ComponentInventoryMovement
{
    public int Id { get; set; }
    
    public int ComponentId { get; set; }
    public Component Component { get; set; } = null!;

    public decimal QuantityChange { get; set; }
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
    
    public int? ReceiveOrderRowId { get; set; }
    public int? ManualAdjustmentId { get; set; }
    
    public ReceiveOrderRow? ReceiveOrderRow { get; set; }
    public ManualAdjustment? ManualAdjustment { get; set; }

}