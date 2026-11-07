using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Repositories;
using JewelryInventory.Api.Services;
using NSubstitute;

namespace JewelryInventory.Tests.Services;

public class ComponentInventoryMovementServiceTests
{
    private readonly IComponentRepository _componentRepository =
        Substitute.For<IComponentRepository>();

    private readonly IComponentInventoryMovementRepository _movementRepository =
        Substitute.For<IComponentInventoryMovementRepository>();

    private readonly ComponentInventoryMovementService _service;

    public ComponentInventoryMovementServiceTests()
    {
        _service = new ComponentInventoryMovementService(
            _componentRepository,
            _movementRepository);
    }

    [Fact]
    public async Task GetComponentInventoryMovementsAsync_ComponentDoesNotExist_ReturnsNull()
    {
        // Arrange
        int componentId = 1;

        _componentRepository.ComponentExistsAsync(componentId).Returns(false);
        
        // Act
        var movements = await _service.GetComponentInventoryMovementsAsync(
            componentId);
        
        // Assert
        Assert.Null(movements);

        await _movementRepository.DidNotReceive()
            .GetComponentInventoryMovementsAsync(
                Arg.Any<int>(),
                Arg.Any<string?>());
    }

    [Fact]
    public async Task GetComponentInventoryMovementAsync_InvalidMovementSource_ThrowsArgumentException()
    {
        // Arrange
        int componentId = 3;
        string source = "invalidsource";

        _componentRepository.ComponentExistsAsync(componentId)
            .Returns(true);
        
        // Act
        var exception =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.GetComponentInventoryMovementsAsync(componentId, source));

        // Assert
        Assert.Equal("Invalid movement source", exception.Message);

        await _movementRepository.DidNotReceive()
            .GetComponentInventoryMovementsAsync(
                Arg.Any<int>(),
                Arg.Any<string?>());
    }

    [Fact]
    public async Task GetComponentInventoryMovementAsync_ValidRequest_ReturnsInventoryMovements()
    {
        // Arrange
        int componentId = 1;

        var expectedMovements = new List<ComponentInventoryMovementDto>
        {
            new ComponentInventoryMovementDto(),
            new ComponentInventoryMovementDto()
        };

        _componentRepository.ComponentExistsAsync(componentId).Returns(true);

        _movementRepository
            .GetComponentInventoryMovementsAsync(componentId, null)
            .Returns(expectedMovements);

        // Act
        var movements =
            await _service.GetComponentInventoryMovementsAsync(componentId);
        
        // Assert
        Assert.Same(expectedMovements, movements);

        await _movementRepository.Received(1)
            .GetComponentInventoryMovementsAsync(componentId, null);

    }
}