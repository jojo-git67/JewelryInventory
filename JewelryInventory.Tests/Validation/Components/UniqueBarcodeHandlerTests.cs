using JewelryInventory.Api.Exceptions;
using JewelryInventory.Api.Repositories;
using JewelryInventory.Api.Validation.Components;
using NSubstitute;

namespace JewelryInventory.Tests.Validation.Components;

public class UniqueBarcodeHandlerTests
{
    private readonly IComponentRepository _componentRepository =
        Substitute.For<IComponentRepository>();

    private readonly UniqueBarcodeHandler _handler;

    public UniqueBarcodeHandlerTests()
    {
        _handler = new UniqueBarcodeHandler(_componentRepository);
    }

    [Fact]
    public async Task HandleAsync_BarcodeExists_ThrowsDuplicateBarcodeException()
    {
        _componentRepository.BarcodeExistsAsync("123", null).Returns(true);

        await Assert.ThrowsAsync<DuplicateBarcodeException>(() =>
            _handler.HandleAsync(new ComponentValidationContext { Barcode = "123" }));
    }

    [Fact]
    public async Task HandleAsync_NullBarcode_SkipsRepositoryCall()
    {
        await _handler.HandleAsync(new ComponentValidationContext { Barcode = null });

        await _componentRepository.DidNotReceiveWithAnyArgs().BarcodeExistsAsync(default!, default);
    }
}