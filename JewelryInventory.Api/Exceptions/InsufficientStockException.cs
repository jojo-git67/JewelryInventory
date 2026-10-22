namespace JewelryInventory.Api.Exceptions;

public class InsufficientStockException : Exception
{
    public InsufficientStockException(int componentId)
        : base($"Insufficient stock for component with ID {componentId}.") { }
}