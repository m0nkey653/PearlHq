namespace PearlHqWeb.Models;

public class TareEventRequest
{
    public int DeviceId { get; set; }
    public int DishId { get; set; }
    public double RawWeightGrams { get; set; }
}
