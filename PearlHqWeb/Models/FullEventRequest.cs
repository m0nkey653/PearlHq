namespace PearlHqWeb.Models;

public class FullEventRequest
{
    public int DeviceId { get; set; }
    public int DishId { get; set; }
    public double RawWeightGrams { get; set; }
}
