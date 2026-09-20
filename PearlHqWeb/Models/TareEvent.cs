using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PearlHqWeb.Models;

public class TareEvent
{
    public int Id { get; set; }

    public int DishId { get; set; }

    [ValidateNever]
    public Dish Dish { get; set; } = default!;

    public double RawWeightGrams { get; set; }
    public DateTime Timestamp { get; set; }
}
