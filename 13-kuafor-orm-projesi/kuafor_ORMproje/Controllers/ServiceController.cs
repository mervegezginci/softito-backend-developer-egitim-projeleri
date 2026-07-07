using kuafor_ORMproje.Data.Repository.IRepository;
using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Mvc;
using System;

namespace kuafor_ORMproje.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Service
        public IActionResult Index()
        {
            var services = _unitOfWork.Service.GetAll();
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
        public IActionResult Create([Bind("Id,ServiceName,Price,Duration,Description,ImageUrl")] Service service)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(service.ImageUrl))
                {
                    service.ImageUrl = "/img/default-service.jpg";
                }
                _unitOfWork.Service.Add(service);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Hizmet başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }

        // GET: Service/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var service = _unitOfWork.Service.GetFirstOrDefault(u => u.Id == id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }

        // POST: Service/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,ServiceName,Price,Duration,Description,ImageUrl")] Service service)
        {
            if (id != service.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(service.ImageUrl))
                {
                    service.ImageUrl = "/img/default-service.jpg";
                }
                _unitOfWork.Service.Update(service);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Hizmet başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }

        // POST: Service/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var service = _unitOfWork.Service.GetFirstOrDefault(u => u.Id == id);
            if (service != null)
            {
                _unitOfWork.Service.Remove(service);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Hizmet başarıyla silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
