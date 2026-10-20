namespace JewelryInventory.Api.Models;

public class ReceiveOrderRow
{
    public int Id { get; set; }
    
    public int ReceiveOrderId { get; set; }
    public ReceiveOrder ReceiveOrder { get; set; } = null!;

    public int ComponentId { get; set; }
    public Component Component { get; set; } = null!;
    
    public decimal QuantityReceived { get; set; }
    public decimal UnitPurchasePrice { get; set; }
    
    public ComponentInventoryMovement?  InventoryMovement { get; set; }
}