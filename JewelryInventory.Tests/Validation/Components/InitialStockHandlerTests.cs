using JewelryInventory.Api.Validation.Components;

namespace JewelryInventory.Tests.Validation.Components;

public class InitialStockHandlerTests
{
    private readonly InitialStockHandler _handler = new();

    [Theory]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public async Task HandleAsync_NegativeInitialStock_ThrowsException(double value)
    {
        // Arrange
        var context = new ComponentValidationContext { InitialStock = (decimal) value };
        
        // Act
        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.HandleAsync(context));
        
        // Assert
        Assert.Equal("Initial stock cannot be negative", ex.Message);
    }

    [Fact]
    public async Task HandleAsync_NullInitialStock_DoesNotThrow()
    {
        await _handler.HandleAsync(new ComponentValidationContext { InitialStock = null });
    }
}