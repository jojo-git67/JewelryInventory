namespace JewelryInventory.Api.Validation.Components;

public abstract class ComponentValidationHandler : IComponentValidationHandler
{
    private IComponentValidationHandler _next;
    
    public IComponentValidationHandler SetNext(IComponentValidationHandler next)
    {
        _next = next;
        return next;
    }

    public async Task HandleAsync(ComponentValidationContext context)
    {
        await ValidateAsync(context);

        if (_next is not null)
        {
            await _next.HandleAsync(context);
        }
    }
    
    protected abstract Task ValidateAsync(ComponentValidationContext context);
}