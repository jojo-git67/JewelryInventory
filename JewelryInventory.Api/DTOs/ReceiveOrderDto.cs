namespace JewelryInventory.Api.DTOs;

public class ReceiveOrderDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<ReceiveOrderRowDto> Rows { get; set; } = new();
}