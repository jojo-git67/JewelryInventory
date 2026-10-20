using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Services;
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
}