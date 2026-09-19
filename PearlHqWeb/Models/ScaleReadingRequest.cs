namespace PearlHqWeb.Models;

public class ScaleReadingRequest
{
    public int DeviceId { get; set; }
    public int DishId { get; set; }
    public double WeightGrams { get; set; }
}
