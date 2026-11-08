using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Exceptions;
using JewelryInventory.Api.Models;
using JewelryInventory.Api.Repositories;
using JewelryInventory.Api.Services;
using JewelryInventory.Api.Validation.Components;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.ExceptionExtensions;

namespace JewelryInventory.Tests.Services;

public class ComponentServiceTests
{
    private readonly IComponentRepository _componentRepository =
        Substitute.For<IComponentRepository>();

    private readonly IComponentInventoryMovementRepository _movementRepository =
        Substitute.For<IComponentInventoryMovementRepository>();

    private readonly IUnitOfWork _unitOfWork =
        Substitute.For<IUnitOfWork>();

    private readonly IComponentValidationHandler _validation =
        Substitute.For<IComponentValidationHandler>();

    private readonly ComponentService _service;
        

    public ComponentServiceTests()
    {
        _service = new ComponentService(
            _componentRepository,
            _movementRepository,
            _unitOfWork,
            _validation);
    }

    [Fact]
    public async Task CreateComponentAsync_ValidationFails_DoesNotSaveAnything()
    {
        var request = new CreateComponentRequest
        {
            Name = "x",
            UnitOfMeasurement = "pcs"
        };

        _validation.HandleAsync(Arg.Any<ComponentValidationContext>())
            .ThrowsAsync(new ArgumentException("boom"));

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateComponentAsync(request));
        
        _movementRepository.DidNotReceive().Add(Arg.Any<ComponentInventoryMovement>());
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