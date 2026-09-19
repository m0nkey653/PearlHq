using System.ComponentModel.DataAnnotations;

namespace PearlHqWeb.Models;

public class Dish
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    public DishType Type { get; set; }

    public int DeviceId { get; set; }
    public Device Device { get; set; } = default!;

    public double? EmptyWeightGrams { get; set; }
    public double? LowThresholdGrams { get; set; }
}
