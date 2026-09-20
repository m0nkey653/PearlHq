using InfluxDB.Client;
using InfluxDB.Client.Writes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
    private readonly InfluxDBClient _influxClient;
    private readonly InfluxOptions _influxOptions;

    public TareEventsController(
        ILogger<TareEventsController> logger,
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
    public async Task<IActionResult> Post([FromBody] TareEventRequest request)
    {
        var dish = await _db.Dishes.FindAsync(request.DishId);
        if (dish is null || dish.DeviceId != request.DeviceId)
        {
            return NotFound($"No dish {request.DishId} for device {request.DeviceId}");
        }

        var timestamp = DateTime.UtcNow;

        _db.TareEvents.Add(new TareEvent
        {
            DishId = dish.Id,
            RawWeightGrams = request.RawWeightGrams,
            Timestamp = timestamp
        });

        dish.EmptyWeightGrams = request.RawWeightGrams;

        await _db.SaveChangesAsync();

        var point = PointData
            .Measurement("tare_event")
            .Tag("deviceId", request.DeviceId.ToString())
            .Tag("dishId", request.DishId.ToString())
            .Field("emptyWeightGrams", request.RawWeightGrams)
            .Timestamp(timestamp, InfluxDB.Client.Api.Domain.WritePrecision.Ns);

        using (var writeApi = _influxClient.GetWriteApi())
        {
            writeApi.WritePoint(point, _influxOptions.Bucket, _influxOptions.Org);
        }

        _logger.LogInformation(
            "Recorded tare event: device={DeviceId} dish={DishId} emptyWeight={RawWeightGrams}g",
            request.DeviceId, request.DishId, request.RawWeightGrams);

        return Ok(new { dish.Id, dish.EmptyWeightGrams });
    }
}
