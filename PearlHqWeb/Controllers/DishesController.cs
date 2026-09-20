using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PearlHqWeb.Data;
using PearlHqWeb.Models;

namespace PearlHqWeb.Controllers;

public class DishesController : Controller
{
    private readonly PearlHqDb _db;

    public DishesController(PearlHqDb db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _db.Dishes.Include(d => d.Device).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();

        var dish = await _db.Dishes.Include(d => d.Device).FirstOrDefaultAsync(d => d.Id == id);
        if (dish is null) return NotFound();

        return View(dish);
    }

    public IActionResult Create()
    {
        ViewData["DeviceId"] = new SelectList(_db.Devices, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Type,DeviceId,LowThresholdGrams,TargetFullWeightGrams")] Dish dish)
    {
        if (!ModelState.IsValid)
        {
            ViewData["DeviceId"] = new SelectList(_db.Devices, "Id", "Name", dish.DeviceId);
            return View(dish);
        }

        _db.Add(dish);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();

        var dish = await _db.Dishes.FindAsync(id);
        if (dish is null) return NotFound();

        ViewData["DeviceId"] = new SelectList(_db.Devices, "Id", "Name", dish.DeviceId);
        return View(dish);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type,DeviceId,LowThresholdGrams,TargetFullWeightGrams")] Dish dish)
    {
        if (id != dish.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewData["DeviceId"] = new SelectList(_db.Devices, "Id", "Name", dish.DeviceId);
            return View(dish);
        }

        // EmptyWeightGrams is device-owned (set only via a reported tare event), so it's
        // deliberately excluded from the bound fields above and left untouched here.
        var existing = await _db.Dishes.FindAsync(id);
        if (existing is null) return NotFound();

        existing.Name = dish.Name;
        existing.Type = dish.Type;
        existing.DeviceId = dish.DeviceId;
        existing.LowThresholdGrams = dish.LowThresholdGrams;
        existing.TargetFullWeightGrams = dish.TargetFullWeightGrams;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();

        var dish = await _db.Dishes.Include(d => d.Device).FirstOrDefaultAsync(d => d.Id == id);
        if (dish is null) return NotFound();

        return View(dish);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var dish = await _db.Dishes.FindAsync(id);
        if (dish is not null)
        {
            _db.Dishes.Remove(dish);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
