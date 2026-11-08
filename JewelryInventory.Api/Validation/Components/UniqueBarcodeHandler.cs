using JewelryInventory.Api.Exceptions;
using JewelryInventory.Api.Repositories;

namespace JewelryInventory.Api.Validation.Components;

public class UniqueBarcodeHandler : ComponentValidationHandler
{
    private readonly IComponentRepository _componentRepository;
    
    public UniqueBarcodeHandler(IComponentRepository componentRepository)
    {
        _componentRepository = componentRepository;
    }
    
    protected override async Task ValidateAsync(ComponentValidationContext context)
    {
        if (context.Barcode is null) return;

        if (await _componentRepository.BarcodeExistsAsync(context.Barcode, context.ComponentId))
            throw new DuplicateBarcodeException(context.Barcode);
    }
}