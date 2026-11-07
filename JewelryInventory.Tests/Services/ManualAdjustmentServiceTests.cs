using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Exceptions;
using JewelryInventory.Api.Models;
using JewelryInventory.Api.Repositories;
using JewelryInventory.Api.Services;
using NSubstitute;

namespace JewelryInventory.Tests.Services;

public class ManualAdjustmentServiceTests
{
    private readonly IComponentRepository _componentRepository =
        Substitute.For<IComponentRepository>();

    private readonly IComponentInventoryMovementRepository _movementRepository =
        Substitute.For<IComponentInventoryMovementRepository>();
    
    private readonly IManualAdjustmentRepository _manualAdjustmentRepository =
        Substitute.For<IManualAdjustmentRepository>();

    private readonly InventoryLockService _inventoryLockService = new();

    private readonly IUnitOfWork _unitOfWork =
        Substitute.For<IUnitOfWork>();

    private readonly ManualAdjustmentService _service;

    public ManualAdjustmentServiceTests()
    {
        _service = new ManualAdjustmentService(
            _componentRepository,
            _movementRepository,
            _manualAdjustmentRepository,
            _unitOfWork,
            _inventoryLockService
            );
    }

    [Fact]
    public async Task CreateManualAdjustmentAsync_InventoryCannotBeNegative_ThrowsInsufficientStockException()
    {
        // Arrange
        int componentId = 1;
        
        var request = new CreateManualAdjustmentRequest()
        {
            Reason = "Damaged component",
            QuantityChange = -100,
        };

        _componentRepository
            .ComponentExistsAsync(componentId)
            .Returns(true);
        
        _componentRepository
            .GetEntityByIdAsync(componentId)
            .Returns(
                new Component
                {
                    Id = componentId,
                    Name = "Test Component",
                });

        _movementRepository
            .GetCurrentStockAsync(componentId)
            .Returns(50);

        // Act
        var exception = await Assert.ThrowsAsync<InsufficientStockException>(
            () => _service.CreateManualAdjustmentAsync(componentId, request));
        
        // Assert
        Assert.NotNull(exception);
        
        _movementRepository.DidNotReceive().Add(Arg.Any<ComponentInventoryMovement>());

        await _unitOfWork.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task CreateManualAdjustmentAsync_RequestQuantityChangeIsZero_ThrowsArgumentException()
    {
        // Arrange
        int componentId = 1;
        
        var request = new CreateManualAdjustmentRequest
        {
            Reason = "Damaged component",
            QuantityChange = 0,
        };
        
        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateManualAdjustmentAsync(componentId, request));

        // Assert
        Assert.NotNull(exception);
        
        Assert.Equal("Quantity change cannot be 0", exception.Message);
    }

    [Fact]
    public async Task CreateManualAdjustmentAsync_ValidRequest_CreatesComponentInventoryMovement()
    {
        // Arrange
        int componentId = 1;

        var request = new CreateManualAdjustmentRequest
        {
            Reason = "Damaged Component",
            QuantityChange = -10,
        };

        _componentRepository.ComponentExistsAsync(componentId)
            .Returns(true);

        _componentRepository.GetEntityByIdAsync(componentId)
            .Returns(
                new Component
                {
                    Id = componentId,
                    Name = "Test Component",
                });

        _movementRepository.GetCurrentStockAsync(componentId)
            .Returns(50);
        
        // Act
        var movement = await _service.CreateManualAdjustmentAsync(
            componentId, request);
        
        // Assert
        Assert.NotNull(movement);
        
        _movementRepository.Received(1)
            .Add(Arg.Any<ComponentInventoryMovement>());
        
        await _unitOfWork.Received(1).SaveChangesAsync();
    }
}