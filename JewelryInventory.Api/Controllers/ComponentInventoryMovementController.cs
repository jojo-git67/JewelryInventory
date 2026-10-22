using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Models;
using JewelryInventory.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace JewelryInventory.Api.Controllers;

[ApiController]
[Route("api/components/{componentId:int}/movements")]
public class ComponentInventoryMovementController : ControllerBase
{
    private readonly ComponentInventoryMovementService _service;
    
    public  ComponentInventoryMovementController(
        ComponentInventoryMovementService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<ComponentInventoryMovementDto>>> GetMovements(
        int componentId,
        [FromQuery] string? source = null)
    {
        try
        {
            var movements = await _service.GetComponentInventoryMovementsAsync(
                componentId, source);

            if (movements is null)
            {
                return NotFound();
            }

            return Ok(movements);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}