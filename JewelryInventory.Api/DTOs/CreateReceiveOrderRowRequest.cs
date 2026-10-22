namespace JewelryInventory.Api.DTOs;


public class CreateReceiveOrderRowRequest
{
    public int ComponentId { get; set; }
    public decimal QuantityReceived { get; set; }
    public decimal UnitPurchasePrice { get; set; }
}