namespace JewelryInventory.Api.DTOs;

public class CreateReceiveOrderRequest
{
    public List<CreateReceiveOrderRowRequest> Rows { get; set; } = new();
}