using Microsoft.AspNetCore.Mvc;
using PearlHqWeb.Data;
using PearlHqWeb.Services;

namespace PearlHqWeb.Controllers;

[ApiController]
[Route("api/dishes")]
public class DishStatusController : ControllerBase
{
    private readonly PearlHqDb _db;
    private readonly DishStatusService _statusService;

    public DishStatusController(PearlHqDb db, DishStatusService statusService)
    {
        _db = db;
        _statusService = statusService;
    }

    [HttpGet("{dishId}/status")]
    public async Task<IActionResult> GetStatus(int dishId)
    {
        var dish = await _db.Dishes.FindAsync(dishId);
        if (dish is null)
        {
            return NotFound($"No dish {dishId}");
        }

        var status = await _statusService.GetLatestAsync(dish);
        return Ok(status);
    }
}
