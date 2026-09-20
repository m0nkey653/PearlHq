using InfluxDB.Client;
using InfluxDB.Client.Writes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PearlHqWeb.Data;
using PearlHqWeb.Filters;
using PearlHqWeb.Models;
using PearlHqWeb.Services;

namespace PearlHqWeb.Controllers;

[ApiController]
[Route("api/scale-readings")]
[ApiKeyAuth]
public class ScaleReadingsController : ControllerBase
{
    private readonly ILogger<ScaleReadingsController> _logger;
    private readonly InfluxDBClient _influxClient;
    private readonly InfluxOptions _influxOptions;
    private readonly PearlHqDb _db;
    private readonly DishStatusService _statusService;

    public ScaleReadingsController(
        ILogger<ScaleReadingsController> logger,
        InfluxDBClient influxClient,
        IOptions<InfluxOptions> influxOptions,
        PearlHqDb db,
        DishStatusService statusService)
    {
        _logger = logger;
        _influxClient = influxClient;
        _influxOptions = influxOptions.Value;
        _db = db;
        _statusService = statusService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ScaleReadingRequest reading)
    {
        var dish = await _db.Dishes.FindAsync(reading.DishId);
        if (dish is null || dish.DeviceId != reading.DeviceId)
        {
            return NotFound($"No dish {reading.DishId} for device {reading.DeviceId}");
        }

        var point = PointData
            .Measurement("scale_reading")
            .Tag("deviceId", reading.DeviceId.ToString())
            .Tag("dishId", reading.DishId.ToString())
            .Tag("dishType", dish.Type.ToString())
            .Field("weightGrams", reading.WeightGrams)
            .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ns);

        using (var writeApi = _influxClient.GetWriteApi())
        {
            writeApi.WritePoint(point, _influxOptions.Bucket, _influxOptions.Org);
        }

        _logger.LogInformation(
            "Recorded scale reading: device={DeviceId} dish={DishId} weight={WeightGrams}g",
            reading.DeviceId, reading.DishId, reading.WeightGrams);

        var status = _statusService.Compute(dish, reading.WeightGrams);
        return Ok(status);
    }
}
