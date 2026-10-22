using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Exceptions;
using JewelryInventory.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace JewelryInventory.Api.Controllers;

[ApiController]
[Route("api/components/{componentId:int}/adjustments")]
public class ManualAdjustmentsController : ControllerBase
{
    private readonly ManualAdjustmentService _manualAdjustmentService;

    public ManualAdjustmentsController(ManualAdjustmentService manualAdjustmentService)
    {
        _manualAdjustmentService = manualAdjustmentService;
    }

    [HttpPost]
    public async Task<ActionResult<ManualAdjustmentDto>> CreateAdjustment(
        int componentId,
        [FromBody] CreateManualAdjustmentRequest request)
    {
        try
        {
            var adjustment = await _manualAdjustmentService.
                CreateManualAdjustmentAsync(
                    componentId, 
                    request);
            
            if (adjustment is null)
            {
                return NotFound();
            }
        
            return StatusCode(StatusCodes.Status201Created, adjustment);
        }
        catch (InsufficientStockException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}