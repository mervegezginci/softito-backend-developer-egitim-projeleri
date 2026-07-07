using kuafor_ORMproje.Data.Repository.IRepository;
using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace kuafor_ORMproje.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Customer
        public IActionResult Index()
        {
            var customers = _unitOfWork.Customer.GetAll().OrderByDescending(c => c.CreatedDate).ToList();
            return View(customers);
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,FullName,Phone,Email,Gender")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.CreatedDate = DateTime.Now;
                _unitOfWork.Customer.Add(customer);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Müşteri başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customer/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var customer = _unitOfWork.Customer.GetFirstOrDefault(u => u.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,FullName,Phone,Email,Gender,CreatedDate")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Customer.Update(customer);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Müşteri bilgileri güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var customer = _unitOfWork.Customer.GetFirstOrDefault(u => u.Id == id);
            if (customer != null)
            {
                _unitOfWork.Customer.Remove(customer);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Müşteri başarıyla silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
