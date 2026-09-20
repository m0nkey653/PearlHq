using InfluxDB.Client;
using Microsoft.Extensions.Options;
using PearlHqWeb.Data;
using PearlHqWeb.Models;

namespace PearlHqWeb.Services;

public class DishStatusService
{
    private readonly InfluxDBClient _influxClient;
    private readonly InfluxOptions _influxOptions;

    public DishStatusService(InfluxDBClient influxClient, IOptions<InfluxOptions> influxOptions)
    {
        _influxClient = influxClient;
        _influxOptions = influxOptions.Value;
    }

    public DishStatusResult Compute(Dish dish, double rawWeightGrams)
    {
        if (dish.EmptyWeightGrams is not { } emptyWeightGrams)
        {
            return new DishStatusResult(dish.Id, rawWeightGrams, null, DishStatus.Unknown, null);
        }

        var contentWeightGrams = rawWeightGrams - emptyWeightGrams;

        var status = dish.LowThresholdGrams is { } lowThreshold && contentWeightGrams <= lowThreshold
            ? DishStatus.Low
            : DishStatus.Ok;

        double? suggestedRefillGrams = null;
        if (status == DishStatus.Low && dish.TargetFullWeightGrams is { } targetFullWeightGrams)
        {
            suggestedRefillGrams = Math.Max(0, targetFullWeightGrams - contentWeightGrams);
        }

        return new DishStatusResult(dish.Id, rawWeightGrams, contentWeightGrams, status, suggestedRefillGrams);
    }

    public async Task<DishStatusResult> GetLatestAsync(Dish dish)
    {
        var flux = $$"""
            from(bucket: "{{_influxOptions.Bucket}}")
              |> range(start: -30d)
              |> filter(fn: (r) => r._measurement == "scale_reading" and r.dishId == "{{dish.Id}}" and r._field == "weightGrams")
              |> last()
            """;

        var tables = await _influxClient.GetQueryApi().QueryAsync(flux, _influxOptions.Org);
        var record = tables.SelectMany(t => t.Records).FirstOrDefault();

        if (record is null)
        {
            return new DishStatusResult(dish.Id, null, null, DishStatus.Unknown, null);
        }

        var rawWeightGrams = Convert.ToDouble(record.GetValue());
        return Compute(dish, rawWeightGrams);
    }
}
