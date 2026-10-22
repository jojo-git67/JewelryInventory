namespace JewelryInventory.Api.DTOs;

public class CreateManualAdjustmentRequest
{
    public string Reason { get; set; } = string.Empty;
    public decimal QuantityChange { get; set; }
}