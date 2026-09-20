using InfluxDB.Client;
using InfluxDB.Client.Writes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PearlHqWeb.Data;
using PearlHqWeb.Filters;
using PearlHqWeb.Models;

namespace PearlHqWeb.Controllers;

[ApiController]
[Route("api/full-events")]
[ApiKeyAuth]
public class FullEventsController : ControllerBase
{
    private readonly ILogger<FullEventsController> _logger;
    private readonly PearlHqDb _db;
    private readonly InfluxDBClient _influxClient;
    private readonly InfluxOptions _influxOptions;

    public FullEventsController(
        ILogger<FullEventsController> logger,
        PearlHqDb db,
        InfluxDBClient influxClient,
        IOptions<InfluxOptions> influxOptions)
    {
        _logger = logger;
        _db = db;
        _influxClient = influxClient;
        _influxOptions = influxOptions.Value;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] FullEventRequest request)
    {
        var dish = await _db.Dishes.FindAsync(request.DishId);
        if (dish is null || dish.DeviceId != request.DeviceId)
        {
            return NotFound($"No dish {request.DishId} for device {request.DeviceId}");
        }

        if (dish.EmptyWeightGrams is not { } emptyWeightGrams)
        {
            return BadRequest($"Dish {request.DishId} has no recorded tare yet — tare before marking full.");
        }

        var timestamp = DateTime.UtcNow;
        var targetFullWeightGrams = request.RawWeightGrams - emptyWeightGrams;

        _db.FullEvents.Add(new FullEvent
        {
            DishId = dish.Id,
            RawWeightGrams = request.RawWeightGrams,
            Timestamp = timestamp
        });

        dish.TargetFullWeightGrams = targetFullWeightGrams;

        await _db.SaveChangesAsync();

        var point = PointData
            .Measurement("full_event")
            .Tag("deviceId", request.DeviceId.ToString())
            .Tag("dishId", request.DishId.ToString())
            .Field("rawWeightGrams", request.RawWeightGrams)
            .Timestamp(timestamp, InfluxDB.Client.Api.Domain.WritePrecision.Ns);

        using (var writeApi = _influxClient.GetWriteApi())
        {
            writeApi.WritePoint(point, _influxOptions.Bucket, _influxOptions.Org);
        }

        _logger.LogInformation(
            "Recorded full event: device={DeviceId} dish={DishId} targetFullWeight={TargetFullWeightGrams}g",
            request.DeviceId, request.DishId, targetFullWeightGrams);

        return Ok(new { dish.Id, dish.TargetFullWeightGrams });
    }
}
