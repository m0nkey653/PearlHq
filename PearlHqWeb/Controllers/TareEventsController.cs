using Microsoft.AspNetCore.Mvc;
using PearlHqWeb.Data;
using PearlHqWeb.Filters;
using PearlHqWeb.Models;

namespace PearlHqWeb.Controllers;

[ApiController]
[Route("api/tare-events")]
[ApiKeyAuth]
public class TareEventsController : ControllerBase
{
    private readonly ILogger<TareEventsController> _logger;
    private readonly PearlHqDb _db;

    public TareEventsController(ILogger<TareEventsController> logger, PearlHqDb db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TareEventRequest request)
    {
        var dish = await _db.Dishes.FindAsync(request.DishId);
        if (dish is null || dish.DeviceId != request.DeviceId)
        {
            return NotFound($"No dish {request.DishId} for device {request.DeviceId}");
        }

        _db.TareEvents.Add(new TareEvent
        {
            DishId = dish.Id,
            RawWeightGrams = request.RawWeightGrams,
            Timestamp = DateTime.UtcNow
        });

        dish.EmptyWeightGrams = request.RawWeightGrams;

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Recorded tare event: device={DeviceId} dish={DishId} emptyWeight={RawWeightGrams}g",
            request.DeviceId, request.DishId, request.RawWeightGrams);

        return Ok(new { dish.Id, dish.EmptyWeightGrams });
    }
}
