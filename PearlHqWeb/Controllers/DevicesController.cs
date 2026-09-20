using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PearlHqWeb.Data;
using PearlHqWeb.Models;

namespace PearlHqWeb.Controllers;

public class DevicesController : Controller
{
    private readonly PearlHqDb _db;

    public DevicesController(PearlHqDb db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _db.Devices.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();

        var device = await _db.Devices
            .Include(d => d.Dishes)
            .FirstOrDefaultAsync(d => d.Id == id);
        if (device is null) return NotFound();

        return View(device);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Location")] Device device)
    {
        if (!ModelState.IsValid) return View(device);

        _db.Add(device);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();

        var device = await _db.Devices.FindAsync(id);
        if (device is null) return NotFound();

        return View(device);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location")] Device device)
    {
        if (id != device.Id) return NotFound();
        if (!ModelState.IsValid) return View(device);

        _db.Update(device);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();

        var device = await _db.Devices.FirstOrDefaultAsync(d => d.Id == id);
        if (device is null) return NotFound();

        return View(device);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var device = await _db.Devices.FindAsync(id);
        if (device is not null)
        {
            _db.Devices.Remove(device);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
