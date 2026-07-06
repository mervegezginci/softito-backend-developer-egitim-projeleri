using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace kuafor_ORMproje.Controllers
{
    public class ServiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ServiceController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Service
        public async Task<IActionResult> Index()
        {
            var services = await _context.Services.ToListAsync();
            return View(services);
        }
        // GET: Service/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: Service/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ServiceName,Price,Duration,Description,ImageUrl")] Service service)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(service.ImageUrl))
                {
                    service.ImageUrl = "/img/default-service.jpg";
                }
                _context.Add(service);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Hizmet başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }
        // GET: Service/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }
        // POST: Service/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ServiceName,Price,Duration,Description,ImageUrl")] Service service)
        {
            if (id != service.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(service.ImageUrl))
                    {
                        service.ImageUrl = "/img/default-service.jpg";
                    }
                    _context.Update(service);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Hizmet başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(service.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }
        // POST: Service/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Hizmet başarıyla silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.Id == id);
        }
    }
}
