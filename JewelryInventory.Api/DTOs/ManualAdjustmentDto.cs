namespace JewelryInventory.Api.DTOs;

public class ManualAdjustmentDto
{
    public int Id { get; set; }
    public int ComponentId { get; set; }
    public decimal QuantityChange { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}