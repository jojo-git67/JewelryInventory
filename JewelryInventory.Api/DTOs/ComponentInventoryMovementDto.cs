namespace JewelryInventory.Api.DTOs;

public class ComponentInventoryMovementDto
{
    public int Id { get; set; }
    public int ComponentId { get; set; }
    public decimal QuantityChange { get; set; }
    public string Source { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}