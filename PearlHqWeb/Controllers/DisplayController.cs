using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PearlHqWeb.Data;
using PearlHqWeb.Models;
using PearlHqWeb.Services;

namespace PearlHqWeb.Controllers;

public class DisplayController : Controller
{
    private readonly PearlHqDb _db;
    private readonly DishStatusService _statusService;

    public DisplayController(PearlHqDb db, DishStatusService statusService)
    {
        _db = db;
        _statusService = statusService;
    }

    public async Task<IActionResult> Index()
    {
        var dishes = await _db.Dishes.Include(d => d.Device).ToListAsync();

        var foodDishes = new List<(Dish, DishStatusResult)>();
        var waterDishes = new List<(Dish, DishStatusResult)>();

        foreach (var dish in dishes)
        {
            var status = await _statusService.GetLatestAsync(dish);
            var target = dish.Type == DishType.Food ? foodDishes : waterDishes;
            target.Add((dish, status));
        }

        return View(new DisplayViewModel(foodDishes, waterDishes));
    }
}
