using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace JewelryInventory.Api.Controllers;

[ApiController]
[Route("api/receive-orders")]
public class ReceiveOrdersController : ControllerBase
{
    private readonly ReceiveOrderService _service;
    
    public ReceiveOrdersController(ReceiveOrderService service)
    {
        _service = service;
    }

    [HttpGet("{orderId:int}")]
    public async Task<ActionResult<ReceiveOrderDto>> GetReceiveOrderById(
        int orderId)
    {
        var order = await _service.GetReceiveOrderByIdAsync(orderId);

        if (order is null)
        {
            return NotFound();
        }
        
        return Ok(order);
    }
    
    [HttpPost]
    public async Task<ActionResult<ReceiveOrderDto>> CreateReceiveOrder(
        [FromBody] CreateReceiveOrderRequest request)
    {
        try
        {
            var receiveOrder = await _service.CreateReceiveOrderAsync(request);

            return CreatedAtAction(
                nameof(GetReceiveOrderById),
                new  { orderId = receiveOrder.Id },
                receiveOrder);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}