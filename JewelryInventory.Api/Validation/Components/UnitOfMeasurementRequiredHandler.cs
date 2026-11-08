namespace JewelryInventory.Api.Validation.Components;

public class UnitOfMeasurementRequiredHandler : ComponentValidationHandler
{
    protected override Task ValidateAsync(ComponentValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(context.UnitOfMeasurement))
            throw new ArgumentException("Unit of measurement is required");
        
        return Task.CompletedTask;
    }
}