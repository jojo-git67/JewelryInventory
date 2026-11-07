using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Exceptions;
using JewelryInventory.Api.Models;
using JewelryInventory.Api.Repositories;
using JewelryInventory.Api.Services;
using NSubstitute;

namespace JewelryInventory.Tests.Services;

public class ComponentServiceTests
{
    private readonly IComponentRepository _componentRepository =
        Substitute.For<IComponentRepository>();

    private readonly IComponentInventoryMovementRepository _movementRepository =
        Substitute.For<IComponentInventoryMovementRepository>();

    private readonly IUnitOfWork _unitOfWork =
        Substitute.For<IUnitOfWork>();

    private readonly ComponentService _service;
        

    public ComponentServiceTests()
    {
        _service = new ComponentService(
            _componentRepository,
            _movementRepository,
            _unitOfWork);
    }
    
    [Fact]
    public async Task CreateComponentAsync_NegativeInitialStock_ThrowsArgumentException()
    {
        // Arrange
        var request = new CreateComponentRequest
        {
            Name = "Unit Test Name",
            UnitOfMeasurement = "pcs",
            InitialStock = -5,
        };
        
        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateComponentAsync(request));
        
        // Assert
        Assert.Equal("Initial stock cannot be negative", exception.Message);

        await _unitOfWork.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task CreateComponentAsync_DuplicateBarcodeExists_ThrowDuplicateBarcodeException()
    {
        // Arrange
        var request = new CreateComponentRequest
        {
            Name = "Unit Test Name",
            UnitOfMeasurement = "pcs",
            Barcode = "123456789",
        };

        _componentRepository.BarcodeExistsAsync(request.Barcode).Returns(true);

        // Act
        var exception = await Assert.ThrowsAsync<DuplicateBarcodeException>(
            () => _service.CreateComponentAsync(request));

        // Assert
        Assert.NotNull(exception);
        
        _movementRepository.DidNotReceive()
            .Add(Arg.Any<ComponentInventoryMovement>());

        await _componentRepository.Received(1)
            .BarcodeExistsAsync(request.Barcode);

        await _unitOfWork.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task CreateComponent_ValidRequest_CreatesInitialInventoryMovement()
    {
        // Arrange
        var request = new CreateComponentRequest
        {
            Name = "Unit Test Name",
            Category = "Beads",
            Color = "Blue",
            LowStockThreshold = 10,
            InitialStock = 0,
            StorageLocation = "A1",
            UnitCost = 2,
            Barcode = null,
            UnitOfMeasurement = "pcs",
        };
        
        var expectedComponent = new ComponentDto
        {
            Name = request.Name,
            Category = request.Category,
            Color = request.Color,
            CurrentStock = 0,
            UnitOfMeasurement = request.UnitOfMeasurement,
        };

        // Calls GetComponentDtoByIdAsync with any id and tells it to the return the expectedComponent
        _componentRepository.GetComponentDtoByIdAsync(Arg.Any<int>()).Returns(expectedComponent);
        
        // Act
        var component = await _service.CreateComponentAsync(request);
        
        // Assert
        Assert.NotNull(component);
        Assert.Equal(request.Name, component.Name);
        Assert.Equal(0, component.CurrentStock);
        
        // Verifies that ComponentService calls Add exactly one time
        // with an inventory movement that:
        // - has a quantity change of 0
        // - has an associated manual adjustment
        // - has 'Initial stock' as reason
        _movementRepository.Received(1).Add(
            Arg.Is<ComponentInventoryMovement>(m =>
                m.QuantityChange == 0 &&
                m.ManualAdjustment != null &&
                m.ManualAdjustment.QuantityChange == 0 &&
                m.ManualAdjustment.Reason == "Initial stock" &&
                m.Component.Name == request.Name
            ));

        // Verifies that the services attempts to commit changes exactly one time
        await _unitOfWork.Received(1).SaveChangesAsync();
    }
    
}