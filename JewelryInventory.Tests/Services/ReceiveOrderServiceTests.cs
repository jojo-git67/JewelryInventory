using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Models;
using JewelryInventory.Api.Repositories;
using JewelryInventory.Api.Services;
using NSubstitute;
using NSubstitute.Core;

namespace JewelryInventory.Tests.Services;

public class ReceiveOrderServiceTests
{
    private readonly IComponentRepository _componentRepository =
        Substitute.For<IComponentRepository>();

    private readonly IComponentInventoryMovementRepository _movementRepository =
        Substitute.For<IComponentInventoryMovementRepository>();

    private readonly IReceiveOrderRepository _receiveOrderRepository =
        Substitute.For<IReceiveOrderRepository>();

    private readonly InventoryLockService _inventoryLockService = new();

    private readonly IUnitOfWork _unitOfWork =
        Substitute.For<IUnitOfWork>();

    private readonly ReceiveOrderService _service;

    public ReceiveOrderServiceTests()
    {
        _service = new ReceiveOrderService(
            _componentRepository,
            _movementRepository,
            _receiveOrderRepository,
            _unitOfWork,
            _inventoryLockService
        );
    }

    [Fact]
    public async Task CreateReceiveOrderAsync_DuplicateComponentIds_ThrowsArgumentException()
    {
        // Arrange
        var request = new CreateReceiveOrderRequest();
        
        request.Rows.Add(
            new CreateReceiveOrderRowRequest { ComponentId = 1, QuantityReceived = 100, UnitPurchasePrice = 1 });
        request.Rows.Add(
            new CreateReceiveOrderRowRequest { ComponentId = 1, QuantityReceived = 20, UnitPurchasePrice = 2});
        
        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateReceiveOrderAsync(request));
        
        // Assert
        Assert.Equal("A receiving order cannot contain duplicates", exception.Message);

        await _unitOfWork.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task CreateReceiveOrderAsync_QuantityReceivedIsZero_ThrowsArgumentException()
    {
        // Arrange
        var request = new CreateReceiveOrderRequest();
        
        request.Rows.Add(
            new CreateReceiveOrderRowRequest { ComponentId = 1, QuantityReceived = 0, UnitPurchasePrice = 2});

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateReceiveOrderAsync(request));
        
        
        // Assert
        Assert.Equal(
            "Quantity must be greater than zero",
            exception.Message);

        await _unitOfWork.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task CreateReceiveOrderAsync_ValidRequest_CreatesComponentInventoryMovements()
    {
        // Arrange
        var request = new CreateReceiveOrderRequest();
        
        request.Rows.Add(
            new CreateReceiveOrderRowRequest { ComponentId = 1, QuantityReceived = 10, UnitPurchasePrice = 1 });
        request.Rows.Add(
            new CreateReceiveOrderRowRequest { ComponentId = 2, QuantityReceived = 10, UnitPurchasePrice = 2 });

        _componentRepository.AllComponentsExistAsync(Arg.Any<IEnumerable<int>>())
            .Returns(true);
        
        // Simulate ReceiveOrderService return statement
        _receiveOrderRepository
            .GetReceiveOrderDtoByIdAsync(Arg.Any<int>())
            .Returns(new ReceiveOrderDto());

        // Act
        var receiveOrder = await _service.CreateReceiveOrderAsync(request);
        
        // Assert
        Assert.NotNull(receiveOrder);
        
        _movementRepository.Received(2)
            .Add(Arg.Any<ComponentInventoryMovement>());
        
        _receiveOrderRepository.Received(1)
            .Add(Arg.Any<ReceiveOrder>());

        await _unitOfWork.Received(1).SaveChangesAsync();

    }
}