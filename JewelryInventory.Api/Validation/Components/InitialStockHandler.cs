namespace JewelryInventory.Api.Validation.Components;

public class InitialStockHandler : ComponentValidationHandler
{
    protected override Task ValidateAsync(ComponentValidationContext context)
    {
        if (context.InitialStock < 0)
            throw new ArgumentException("Initial stock cannot be negative");
        
        return Task.CompletedTask;
    }
}