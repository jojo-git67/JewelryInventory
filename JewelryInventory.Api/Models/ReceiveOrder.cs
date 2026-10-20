namespace JewelryInventory.Api.Models;

public class ReceiveOrder
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
    
    public ICollection<ReceiveOrderRow> ReceiveOrderRows { get; set; } 
        = new List<ReceiveOrderRow>();
}