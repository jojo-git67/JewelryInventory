using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Models;
using JewelryInventory.Api.Repositories;


namespace JewelryInventory.Api.Services;

public class ReceiveOrderService
{
    private readonly IComponentRepository _componentRepository;
    private readonly IComponentInventoryMovementRepository _movementRepository;
    private readonly IReceiveOrderRepository _receiveOrderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly InventoryLockService _inventoryLockService;

    public ReceiveOrderService(
        IComponentRepository componentRepository,
        IComponentInventoryMovementRepository movementRepository,
        IReceiveOrderRepository receiveOrderRepository,
        IUnitOfWork unitOfWork,
        InventoryLockService inventoryLockService)
    {
        _componentRepository = componentRepository;
        _movementRepository = movementRepository;
        _receiveOrderRepository = receiveOrderRepository;
        _unitOfWork = unitOfWork;
        _inventoryLockService = inventoryLockService;
    }

    public async Task<ReceiveOrderDto?> GetReceiveOrderByIdAsync(int orderId)
    {
        return await _receiveOrderRepository.GetReceiveOrderDtoByIdAsync(orderId);
    }

    public async Task<ReceiveOrderDto> CreateReceiveOrderAsync(
        CreateReceiveOrderRequest request)
    {
        if (request.Rows is null || request.Rows.Count == 0)
        {
            throw new ArgumentException("A receiving order must contain at least one row");
        }

        var componentIds = new HashSet<int>();

        foreach (var row in request.Rows)
        {
            if (row is null)
                throw new ArgumentException("Receiving order rows cannot be null.");

            if (row.QuantityReceived <= 0)
                throw new ArgumentException("Quantity must be greater than zero");

            if (row.UnitPurchasePrice < 0)
                throw new ArgumentException("Unit price cannot be negative");
            
            if (!componentIds.Add(row.ComponentId))
            {
                throw new ArgumentException("A receiving order cannot contain duplicates");
            }
        }

        var acquiredLocks = new List<SemaphoreSlim>();

        try
        {
            foreach (var componentId in componentIds.OrderBy(id => id))
            {
                var semaphore = _inventoryLockService.GetComponentLock(componentId);

                await semaphore.WaitAsync();
                acquiredLocks.Add(semaphore);
            }

            if (!await _componentRepository.AllComponentsExistAsync(componentIds))
            {
                throw new ArgumentException("One or more components doesn't exist");
            }

            var now = DateTime.UtcNow;

            var receiveOrder = new ReceiveOrder
            {
                CreatedAt = now,
            };

            foreach (var row in request.Rows)
            {
                var receiveOrderRow = new ReceiveOrderRow
                {
                    ComponentId = row.ComponentId,
                    QuantityReceived = row.QuantityReceived,
                    UnitPurchasePrice = row.UnitPurchasePrice,
                };

                var movement = new ComponentInventoryMovement
                {
                    ComponentId = row.ComponentId,
                    ReceiveOrderRow = receiveOrderRow,
                    QuantityChange = row.QuantityReceived,
                    CreatedAt = now,
                };

                receiveOrder.ReceiveOrderRows.Add(receiveOrderRow);

                _movementRepository.Add(movement);
            }

            _receiveOrderRepository.Add(receiveOrder);

            await _unitOfWork.SaveChangesAsync();

            return await _receiveOrderRepository
                       .GetReceiveOrderDtoByIdAsync(receiveOrder.Id)
                   ?? throw new InvalidOperationException(
                       "The receiving order could not be retrieved after saving");
        }
        finally
        {
            foreach (var semaphore in acquiredLocks)
            {
                semaphore.Release();
            }
        }
    }
}