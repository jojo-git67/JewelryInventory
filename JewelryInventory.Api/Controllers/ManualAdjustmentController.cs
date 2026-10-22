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

    [HttpGet("{adjustmentId:int}")]
    public async Task<ActionResult<ManualAdjustmentDto>> GetManualAdjustmentById(
        int componentId,
        int adjustmentId)
    {
        var adjustment = await _manualAdjustmentService
            .GetManualAdjustmentByIdAsync(componentId, adjustmentId);

        if (adjustment is null)
        {
            return NotFound();
        }

        return Ok(adjustment);
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
        
            return CreatedAtAction(
                nameof(GetManualAdjustmentById),
                new
                {
                    componentId = adjustment.ComponentId,
                    adjustmentId = adjustment.Id
                },
                adjustment
            );
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