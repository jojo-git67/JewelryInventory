namespace JewelryInventory.Api.DTOs;

public class ReceiveOrderRowDto
{
    public int Id { get; set; }
    public int ComponentId { get; set; }
    public decimal QuantityReceived { get; set; }
    public decimal UnitPurchasePrice { get; set; }
}