namespace JewelryInventory.Api.Validation.Components;

public class UnitCostHandler : ComponentValidationHandler
{
    protected override Task ValidateAsync(ComponentValidationContext context)
    {
        if (context.UnitCost < 0)
            throw new ArgumentException("Unit cost cannot be negative");
        
        return Task.CompletedTask;
    }
}