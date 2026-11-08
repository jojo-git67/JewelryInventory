namespace JewelryInventory.Api.Validation.Components;

public class LowStockThresholdHandler : ComponentValidationHandler
{
    protected override Task ValidateAsync(ComponentValidationContext context)
    {
        if (context.LowStockThreshold < 0)
            throw new ArgumentException("Low stock threshold cannot be negative");
        
        return Task.CompletedTask;
    }
}