namespace JewelryInventory.Api.Validation.Components;

public interface IComponentValidationHandler
{
    IComponentValidationHandler SetNext(IComponentValidationHandler next);
    Task HandleAsync(ComponentValidationContext context);
}