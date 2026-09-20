using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PearlHqWeb.Models;

public class Dish
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    public DishType Type { get; set; }

    public int DeviceId { get; set; }

    [ValidateNever]
    public Device Device { get; set; } = default!;

    public double? EmptyWeightGrams { get; set; }
    public double? LowThresholdGrams { get; set; }
    public double? TargetFullWeightGrams { get; set; }
}
