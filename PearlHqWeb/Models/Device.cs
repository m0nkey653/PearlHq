using System.ComponentModel.DataAnnotations;

namespace PearlHqWeb.Models;

public class Device
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    public string? Location { get; set; }

    public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
}
