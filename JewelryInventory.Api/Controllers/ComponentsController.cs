using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Services;
using JewelryInventory.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace JewelryInventory.Api.Controllers;

[ApiController]
[Route("api/components")]
public class ComponentsController : ControllerBase
{
    private readonly ComponentService _componentService;

    public ComponentsController(ComponentService componentService)
    {
        _componentService = componentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ComponentDto>>> GetAllActiveComponents()
    {
        var components = await _componentService.GetAllActiveComponentsAsync();

        return Ok(components);
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<ComponentDto>>> GetAllComponents()
    {
        var components = await _componentService.GetAllComponentsAsync();

        return Ok(components);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ComponentDto>> GetComponentById(int id)
    {
        var component = await _componentService.GetComponentByIdAsync(id);

        if (component is null)
        {
            return NotFound($"Component with ID {id} does not exist");
        }

        return Ok(component);
    }

    [HttpGet("barcode/{barcode}")]
    public async Task<ActionResult<ComponentDto>> GetComponentByBarcode(string barcode)
    {
        var component = await _componentService.GetComponentByBarcodeAsync(barcode);

        if (component is null)
        {
            return NotFound($"Component with Barcode {barcode} does not exist");
        }
        
        return Ok(component);
    }

    [HttpPost]
    public async Task<ActionResult<ComponentDto>> CreateComponent([FromBody] CreateComponentRequest request)
    {
        try
        {
            var component = await _componentService.CreateComponentAsync(request);

            return CreatedAtAction(
                nameof(GetComponentById),
                new { id = component.Id },
                component
            );
        }
        catch (DuplicateBarcodeException e)
        {
            return Conflict(e.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ComponentDto>> UpdateComponent(
        int id,
        [FromBody] UpdateComponentRequest request)
    {
        try
        {
            var component = await _componentService.UpdateComponentAsync(id, request);

            if (component is null)
            {
                return NotFound();
            }
        
            return Ok(component);
        }
        catch (DuplicateBarcodeException e)
        {
            return Conflict(e.Message);
        }
        
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<ComponentDto>> UpdateStatus(
        int id,
        [FromBody] UpdateStatusRequest request)
    {
        var component = await _componentService.SetComponentActiveAsync(
            id, request.IsActive!.Value);

        if (component is null)
        {
            return NotFound();
        } 
        
        return Ok(component);
    }
}
    
