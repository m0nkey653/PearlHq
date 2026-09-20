using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PearlHqWeb.Data;
using PearlHqWeb.Models;
using PearlHqWeb.Services;

namespace PearlHqWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PearlHqDb _db;
    private readonly DishStatusService _statusService;

    public HomeController(ILogger<HomeController> logger, PearlHqDb db, DishStatusService statusService)
    {
        _logger = logger;
        _db = db;
        _statusService = statusService;
    }

    public async Task<IActionResult> Index()
    {
        var dishes = await _db.Dishes.Include(d => d.Device).ToListAsync();

        var statuses = new List<(Dish Dish, DishStatusResult Status)>();
        foreach (var dish in dishes)
        {
            statuses.Add((dish, await _statusService.GetLatestAsync(dish)));
        }

        return View(statuses);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
