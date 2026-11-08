namespace JewelryInventory.Api.Validation.Components;

public class NameRequiredHandler : ComponentValidationHandler
{
    protected override Task ValidateAsync(ComponentValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(context.Name))
            throw new ArgumentException("Component name is required");
        
        return Task.CompletedTask;
    }
}